using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TIZSoft.Utils.Log;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace TIZSoft.AssetManagement
{
    public class AssetBundleUpdater : MonoBehaviour
    {
        //java.net.ConnectException: failed to connect to resource.dev.violin.tizsoft.com/60.250.154.158 (port 80): connect failed: EHOSTUNREACH (No route to host)
        //有機會發生上面的錯誤，之後 webRequest 就卡死了，Progress 不會增加，也不會完成或中斷
        //因為沒有訊息所以做簡單的防卡確認
        //WWW 似乎也有機率發生，待確認
        struct DownloadingReqeust
        {
            public UnityWebRequestAsyncOperation AsyncOperation;
            public WWW WWW;
            public float LastCheckProgressTime;
            public float LastCheckProgressValue;
            public float StartTime;
            
            public string Error
            {
                get
                {
                    if (AsyncOperation != null)
                        return string.IsNullOrEmpty(AsyncOperation.webRequest.error)
                                    ? "None" : AsyncOperation.webRequest.error;
                    if (WWW != null && !object.Equals(WWW, null))
                        return string.IsNullOrEmpty(WWW.error)
                                    ? "None" : WWW.error;

                    return "None";
                }
            }
            public bool HasError
            {
                get
                {
                    if (AsyncOperation != null)
                        return !string.IsNullOrEmpty(AsyncOperation.webRequest.error);
                    if (WWW != null && !object.Equals(WWW, null))
                        return !string.IsNullOrEmpty(WWW.error);

                    return false;
                }
            }
            public bool StuckCheck(
                float timeThreshold = 30,
                //因為 WWW 目前無法用觀察 progress 的情況做防卡，做一個長時間判斷卡住的功能來處理
                float WWWTimeOut = 180)
            {
                if (AsyncOperation != null)
                {
                    if (AsyncOperation.isDone)
                        return false;
                    if (Mathf.Approximately(LastCheckProgressValue, AsyncOperation.progress))
                    {
                        if (Time.realtimeSinceStartup - LastCheckProgressTime
                                > timeThreshold)
                            return true;
                    }
                    else
                    {
                        LastCheckProgressTime = Time.realtimeSinceStartup;
                        LastCheckProgressValue = AsyncOperation.progress;
                    }
                }
                else if (WWW != null && !object.Equals(WWW, null))
                {
                    if (WWW.isDone)
                        return false;

                    if (Time.realtimeSinceStartup > StartTime + WWWTimeOut)
                        return true;
                    
                    //https://issuetracker.unity3d.com/issues/editor-crashes-with-c-plus-plus-runtime-error-when-getting-www-dot-progress-for-a-certain-number-of-times
                    //Fixed in Unity 2018.1
                    //看起來不太一樣，不過有可能是同一個問題

                    //if (Mathf.Approximately(LastCheckProgressValue, WWW.progress))
                    //{
                    //    if (Time.realtimeSinceStartup - LastCheckProgressTime
                    //            > timeThreshold)
                    //        return true;
                    //}
                    //else
                    //{
                    //    LastCheckProgressTime = Time.realtimeSinceStartup;
                    //    LastCheckProgressValue = WWW.progress;
                    //}
                }
                return false;
            }

            public bool IsDone()
            {
                if (AsyncOperation != null)
                    return AsyncOperation.isDone;
                if (WWW != null && !object.Equals(WWW, null))
                    return WWW.isDone;
                return true;
            }
        }
        static readonly Utils.Log.Logger logger = LogManager.Default.FindOrCreateLogger<AssetBundleUpdater>();

        public string RootPath = @"";
        AssetBundleManifest _manifest;
        public AssetBundleManifest Manifest
        {
            get { return _manifest; }
            set
            {
                _manifest = value;
                if (_manifest == null)
                    return;
                assetBundlNames = new HashSet<string>(Manifest.GetAllAssetBundles());
                progress.TotalAmount = assetBundlNames.Count;
                progress.RestAmount = progress.TotalAmount;
                for (int i = 0; i < loadingOperationList.Count; i++)
                {
                    loadingOperationList[i].AsyncOperation.webRequest.Dispose();
                }
                loadingOperationList.Clear();
                IsDone = false;
            }
        }
        public int MaxUpdateFileAmount = 5;
        public bool IsDone { get; protected set; }
        HashSet<string> assetBundlNames;

        List<DownloadingReqeust> loadingOperationList
            = new List<DownloadingReqeust>();

        ReactiveProgress progress = new ReactiveProgress();
        
        public IObservable<ProgressValueSet> ProgressValueSet
        {
            get
            {
                return progress;
            }
        }

        float dumpLoadingProgressTime;

        public bool UseWWW = true;

        float loadingProgress;
        int restFileAmount;

        System.Action loadingAction = null;

        private void Start()
        {
            if (UseWWW)
                loadingAction = FromWWW;
            else
                loadingAction = FromWebRequest;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.realtimeSinceStartup > dumpLoadingProgressTime)
            {
                logger.Debug("Download Status: IsDone: {0}", IsDone);
            }
            //Component 顯示用
            loadingProgress = progress.Progress;
            restFileAmount = progress.RestAmount;

            if (IsDone)
                return;
            if (Manifest == null)
                return;

            if (loadingAction != null)
                loadingAction();


            if (Time.realtimeSinceStartup > dumpLoadingProgressTime)
            {
                dumpLoadingProgressTime = Time.realtimeSinceStartup + 10;
            }
        }
        void FromWWW()
        {
            bool finishedExist = loadingOperationList.Any(request =>
                    request.IsDone() ||
                    request.StuckCheck());
            if (Time.realtimeSinceStartup > dumpLoadingProgressTime)
            {
                var finishedAmount = loadingOperationList.Count(request =>
                    request.IsDone() ||
                    request.StuckCheck());
                logger.Debug("Download Status: Loading:{0},Rest:{1},Finished Amount:{2}"
                    , loadingOperationList.Count
                    , assetBundlNames.Count
                    , finishedAmount);
            }
            if ((loadingOperationList.Count < MaxUpdateFileAmount
                    && assetBundlNames.Count > 0)
                || finishedExist)
            {
                if (finishedExist)
                {
                    var finishedRequests
                        = loadingOperationList.FindAll(request =>
                            request.IsDone() ||
                            request.StuckCheck());

                    if (finishedRequests.Count > 0)
                    {
                        for (int i = 0; i < finishedRequests.Count; i++)
                        {
                            if (finishedRequests[i].WWW != null)
                            {
                                logger.Debug("Downloaded: {0}, Error: {1}",
                                    finishedRequests[i].WWW.url,
                                    finishedRequests[i].Error);

                                if (finishedRequests[i].HasError
                                    || finishedRequests[i].StuckCheck())
                                {
                                    progress.ErrorAmount++;
                                }

                                if (finishedRequests[i].WWW.isDone)
                                {
                                    //立刻釋放 WWW 會造成 Crash
                                    //測試起來應該是需要間隔一段時間再讓 assetBundle 被回收
                                    //或是強制釋放 WWW 的 assetBundle 才會正常
                                    if (finishedRequests[i].WWW.assetBundle)
                                        finishedRequests[i].WWW.assetBundle.Unload(true);
                                }
                            }

                            loadingOperationList.Remove(finishedRequests[i]);
                        }
                        System.GC.Collect();
                        Resources.UnloadUnusedAssets();
                    }
                }
                var seekCount = 0;
                while (
                    assetBundlNames.Count != 0 &&
                    loadingOperationList.Count < MaxUpdateFileAmount)
                {
                    //避免畫面停止更新太久
                    if (seekCount > 100)
                        break;
                    var assetBundleName = assetBundlNames.First();
                    assetBundlNames.Remove(assetBundleName);
                    progress.RestAmount = assetBundlNames.Count;

                    seekCount++;
                    var hash = Manifest.GetAssetBundleHash(assetBundleName);
                    if (Caching.IsVersionCached(assetBundleName, hash))
                    {
                        logger.Debug("Is Up To Date: {0}", assetBundleName);
                        continue;
                    }
                    logger.Debug("Start Download: {0}", assetBundleName);
                    var www =
                        WWW.LoadFromCacheOrDownload(
                        string.Format("{0}?{1}",
                            Path.Combine(RootPath, assetBundleName),
                            Manifest.GetAssetBundleHash(assetBundleName)),
                        Manifest.GetAssetBundleHash(assetBundleName), 0);
                    loadingOperationList.Add(
                        new DownloadingReqeust()
                        {
                            WWW = www,
                            StartTime = Time.realtimeSinceStartup
                        });

                    //單一 frame 只開始一筆下載
                    return;
                }

                if (loadingOperationList.Count == 0
                    && assetBundlNames.Count == 0)
                {
                    IsDone = true;
                    logger.Debug("Update From WWW Completed");
                    progress.OnCompleted();
                }
            }
            else if (loadingOperationList.Count != 0)
            {
                if (Time.realtimeSinceStartup > dumpLoadingProgressTime)
                {
                    logger.Debug("Downloading Amount: {0}", loadingOperationList.Count);
                    foreach (var loadOp in loadingOperationList)
                    {
                        logger.Debug("Downloading : {0}, {1:P2}\n\tWWW Error: {2}",
                            loadOp.WWW.url,
                            loadOp.WWW.progress,
                            loadOp.Error);
                    }
                }
            }
        }
        void FromWebRequest()
        {
            bool finishedExist = loadingOperationList.Any(request =>
                    request.AsyncOperation.isDone ||
                    request.StuckCheck());
            if ((loadingOperationList.Count < MaxUpdateFileAmount
                    && assetBundlNames.Count > 0)
                || finishedExist)
            {
                if (finishedExist)
                {
                    var finishedRequests
                        = loadingOperationList.FindAll(request =>
                            request.AsyncOperation.isDone ||
                            request.StuckCheck());

                    if (finishedRequests.Count > 0)
                    {
                        for (int i = 0; i < finishedRequests.Count; i++)
                        {
                            logger.Debug("Downloaded: {0}, Error: {1}",
                                finishedRequests[i].AsyncOperation.webRequest.url,
                                finishedRequests[i].Error);

                            if (finishedRequests[i].HasError
                                || finishedRequests[i].StuckCheck())
                            {
                                progress.ErrorAmount++;
                            }

                            if (finishedRequests[i].StuckCheck())
                                finishedRequests[i].AsyncOperation.webRequest.Abort();

                            finishedRequests[i].AsyncOperation.webRequest.Dispose();
                            loadingOperationList.Remove(finishedRequests[i]);
                        }
                        System.GC.Collect();
                        Resources.UnloadUnusedAssets();
                    }
                }
                var seekCount = 0;
                while (
                    assetBundlNames.Count != 0 &&
                    loadingOperationList.Count < MaxUpdateFileAmount)
                {
                    //避免畫面停止更新太久
                    if (seekCount > 100)
                        break;
                    var assetBundleName = assetBundlNames.First();
                    assetBundlNames.Remove(assetBundleName);
                    progress.RestAmount = assetBundlNames.Count;

                    seekCount++;
                    var hash = Manifest.GetAssetBundleHash(assetBundleName);
                    if (Caching.IsVersionCached(assetBundleName, hash))
                    {
                        logger.Debug("Is Up To Date: {0}", assetBundleName);
                        continue;
                    }

                    logger.Debug("Start Download: {0}", assetBundleName);
                    //var loadRequest = UnityWebRequest.GetAssetBundle(
                    var loadRequest = UnityWebRequestAssetBundle.GetAssetBundle(
                        string.Format("{0}?{1}",
                            Path.Combine(RootPath, assetBundleName),
                            Manifest.GetAssetBundleHash(assetBundleName)),
                        Manifest.GetAssetBundleHash(assetBundleName), 0);

                    var asyncRequest = loadRequest.SendWebRequest();
                    // UnityWebRequest.isDone 一直 crash 
                    // 不知道為什麼 AssetbundleManager 可以用
                    // 本來以為是 Update 才能用，最後改成單一物件跑 Update
                    // 可是還是 crash
                    // 改用 SendWebRequest() 的 isDone 目前看來是好的
                    loadingOperationList.Add(
                        new DownloadingReqeust() { AsyncOperation = asyncRequest });
                    //單一 frame 只開始一筆下載
                    return;
                }

                if (loadingOperationList.Count == 0
                    && assetBundlNames.Count == 0)
                {
                    IsDone = true;
                    logger.Debug("Update From WebRequest Completed");
                    progress.OnCompleted();
                }
            }
            else if (loadingOperationList.Count != 0)
            {
                if (Time.realtimeSinceStartup > dumpLoadingProgressTime)
                {
                    dumpLoadingProgressTime = Time.realtimeSinceStartup + 10;
                    logger.Debug("Downloading Amount: {0}", loadingOperationList.Count);
                    foreach (var loadOp in loadingOperationList)
                    {
                        logger.Debug("Downloading : {0}, {1:P2}\n\tWebRequest Error: {2}",
                            loadOp.AsyncOperation.webRequest.url,
                            loadOp.AsyncOperation.progress,
                            loadOp.Error);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            progress.OnCompleted();
            foreach (var loadingOp in loadingOperationList)
            {
                if (loadingOp.AsyncOperation != null)
                {
                    loadingOp.AsyncOperation.webRequest.Abort();
                    loadingOp.AsyncOperation.webRequest.Dispose();
                }
            }
            loadingOperationList.Clear();
        }
    }
}
