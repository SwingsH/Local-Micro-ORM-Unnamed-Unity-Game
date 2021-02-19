using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TIZSoft.Log;
using TIZSoft.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using AssetBundles;

namespace TIZSoft.AssetManagement
{
    // TODO 設定方式設計不良，需改善
    /// <summary>
    /// 表示一個遊戲資源管理器。
    /// </summary>
    public class AssetManager : IAssetManager
    {
        static readonly object syncRoot = new object();

        static readonly Log.Logger logger = LogManager.Default.FindOrCreateLogger<AssetManager>();

        /// <summary>
        /// 已經載入好的 assets。
        /// </summary>
        readonly Dictionary<string, FinishedAssetLoadingOperation> loadedAssets = new Dictionary<string, FinishedAssetLoadingOperation>();

        readonly Dictionary<string, AssetLoadingOperation> loadingOperationBacklog = new Dictionary<string, AssetLoadingOperation>();

        /// <summary>
        /// 所有 asset 載入作業。
        /// </summary>
        readonly Dictionary<string, AssetLoadingOperation> loadingOperations = new Dictionary<string, AssetLoadingOperation>();

        /// <summary>
        /// 需要預載的 assets。
        /// </summary>
        readonly List<PreloadAssetLoadingOperation> preloadOperations = new List<PreloadAssetLoadingOperation>();

        /// <summary>
        /// 取得或設定是否要自動初始化。若設為 false，則需要自行呼叫 <see cref="Initialize"/>。
        /// </summary>
        [Obsolete("Use Settings.ShouldInitializeAutomatically instead.")]
        public bool ShouldInitializeAutomatically
        {
            get { return Settings.ShouldInitializeAutomatically; }
            set { Settings.ShouldInitializeAutomatically = value; }
        }

        /// <summary>
        /// 設定是否使用開發版 AssetBundle 伺服器。若為 true，則 SourceType 的設定將會被此設定取代。
        /// </summary>
        [Obsolete("Use Settings.ShouldInitializeAutomatically instead.")]
        public bool IsDevelopmentAssetBundleServer
        {
            get { return Settings.IsDevelopmentAssetBundleServer; }
            set { Settings.IsDevelopmentAssetBundleServer = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源。若 IsDevelopmentAssetBundleServer 為 true，則此設定無效。
        /// </summary>
        [Obsolete("Use Settings.AssetBundleSourceType instead.")]
        public AssetBundleSourceType SourceType
        {
            get { return Settings.AssetBundleSourceType; }
            set { Settings.AssetBundleSourceType = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源的 URL。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 Remote 才有效。
        /// </summary>
        [Obsolete("Use Settings.BaseDownloadingUrl instead.")]
        public string BaseDownloadingUrl
        {
            get { return Settings.BaseDownloadingUrl; }
            set { Settings.BaseDownloadingUrl = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源的相對路徑。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 StreamingAssets 才有效。
        /// </summary>
        [Obsolete("Use Settings.RelativePath instead.")]
        public string RelativePath
        {
            get { return Settings.RelativePath; }
            set { Settings.RelativePath = value; }
        }

        /// <summary>
        /// AssetBundle manifest 檔名。不填的話，則預設為目標平台名稱。
        /// </summary>
        [Obsolete("Use Settings.ManifestAssetBundleName instead.")]
        public string ManifestAssetBundleName
        {
            get { return Settings.ManifestAssetBundleName; }
            set { Settings.ManifestAssetBundleName = value; }
        }
        
        public IReadOnlyReactiveProperty<bool> IsInitialized
        {
            get { return isAssetBundleManagerReady.ToReadOnlyReactiveProperty(); }
        }

        public AssetManagerSettings Settings { get; set; }

        public AssetBundleManager AssetBundleManager { get; private set; }

        readonly int loadingNumber = 1;
        bool isLoading;
        float nextClearUnusedAssetTime;
        readonly BoolReactiveProperty isAssetBundleManagerReady = new BoolReactiveProperty();

        public AssetManager(AssetBundleManager assetBundleManager)
            : this(new AssetManagerSettings(), assetBundleManager)
        {
            // Do nothing
        }

        public AssetManager(AssetManagerSettings settings, AssetBundleManager assetBundleManager)
        {
            ExceptionUtils.VerifyArgumentNull(settings, "settings");
            Settings = settings;
            AssetBundleManager = assetBundleManager;
            if (Settings.ShouldInitializeAutomatically)
            {
                Initialize();
            }
        }
        
        public void Initialize()
        {
            if (IsInitialized.Value)
            {
                logger.Warn("AssetManager is already initialized.");
                return;
            }

            UnitySceneManager.sceneLoaded += UnitySceneManager_SceneLoaded;
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (Time.realtimeSinceStartup > nextClearUnusedAssetTime)
                    {
                        ClearUnusedAssets();
                    }
                });

            AssetBundleManager.logMode = Settings.LogMode;
            AssetBundleManager.SimulateAssetBundleInEditor = Settings.IsSimulationMode;

            if (Settings.IsDevelopmentAssetBundleServer)
            {
                AssetBundleManager.SetDevelopmentAssetBundleServer();
            }
            else
            {
                switch (Settings.AssetBundleSourceType)
                {
                    case AssetBundleSourceType.StreamingAssets:
                        var platformRelativePath = Path.Combine(Settings.RelativePath, Utility.GetPlatformName()).Replace("\\", "/");
                        AssetBundleManager.SetSourceAssetBundleDirectory(platformRelativePath);
                        break;

                    default:
                        AssetBundleManager.SetSourceAssetBundleURL(Settings.BaseDownloadingUrl);
                        break;
                }
            }

            // 有 manifest 需要初始化就下載 manifest，沒的話就直接完成初始化作業。
            var manifestLoadingOperation = string.IsNullOrEmpty(Settings.ManifestAssetBundleName)
                ? AssetBundleManager.Initialize()
                : AssetBundleManager.Initialize(Settings.ManifestAssetBundleName);
            if (manifestLoadingOperation != null)
            {
                manifestLoadingOperation.ToObservable()
                    .DoOnCompleted(OnInitializeDone)
                    .Subscribe();
            }
            else
            {
                OnInitializeDone();
            }
        }
        
        void OnInitializeDone()
        {
            isAssetBundleManagerReady.SetValueAndForceNotify(true);
            CheckStartDownload();
        }

        void UnitySceneManager_SceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            nextClearUnusedAssetTime = Time.realtimeSinceStartup + 5;
        }

        void CheckStartDownload()
        {
            if (!isAssetBundleManagerReady.Value ||
                isLoading ||
                loadingOperationBacklog.Count == 0 && loadingOperations.Count == 0)
            {
                return;
            }
            Observable.FromMicroCoroutine(LoadAssetsAsync).Subscribe();
        }

        IEnumerator LoadAssetsAsync()
        {
            isLoading = true;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            yield return null;

            var finishedBundleNames = new List<string>();

            // 如果還有 assets 要載入，就繼續執行，否則結束。
            while (loadingOperationBacklog.Count > 0 || loadingOperations.Count > 0)
            {
                FlushLoadingBacklog();

                int count = 0;
                finishedBundleNames.Clear();
                string assetBundleName = string.Empty;
                try
                { 
                    foreach (var pair in loadingOperations)
                    {
                        assetBundleName = pair.Key;
                        var assetOperation = pair.Value;

                        if (assetOperation.assetBundleLoadOperation == null)
                        {
                            assetOperation.assetBundleLoadOperation = AssetBundleManager.LoadAssetAsync(
                                assetOperation.AssetBundleName, assetOperation.AssetName, assetOperation.AssetType);
                        }

                        if (assetOperation.assetBundleLoadOperation == null || assetOperation.assetBundleLoadOperation.IsDone())
                        {
                            finishedBundleNames.Add(pair.Key);
                            AssetOperateAsync_OnFinished(pair.Value);
                        }
                        else
                        {
                            count++;
                        }

                        if (count >= loadingNumber)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Check {0} Error", assetBundleName);
                }

                if (finishedBundleNames.Count > 0)
                {
                    foreach (string key in finishedBundleNames)
                    {
                        loadingOperations.Remove(key);
                    }

                    // 有完成的 asset 就確認一次預載清單
                    PreloadOperations_OnAssetLoaded();
                }
                yield return null;
            }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            isLoading = false;
        }

        public AssetLoadingOperation LoadAssetAsync(string assetBundleName)
        {
            return LoadAssetAsync(assetBundleName, string.Empty, typeof(Object));
        }

        public AssetLoadingOperation LoadAssetAsync(string assetBundleName, string assetName)
        {
            return LoadAssetAsync(assetBundleName, assetName, typeof(Object));
        }

        public AssetLoadingOperation LoadAssetAsync(string assetBundleName, string assetName, Type type)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(assetBundleName);
            }

            logger.Trace("Start load asset assetBundleName={0}, assetName={1}", assetBundleName, assetName);

            FinishedAssetLoadingOperation result;
            if (loadedAssets.TryGetValue(assetBundleName, out result))
            {
                if (result.IsAlive)
                {
                    if (result.MainAsset.IsAlive &&
                        result.MainAsset.Target != null &&
                        !result.MainAsset.Target.Equals(null))
                    {
                        var asset = result.MainAsset.Target;
                        logger.Trace("Cached asset is alive. assetBundleName={0}, assetName={1}, assetType={2}, asset={3}",
                                     assetBundleName, assetName, asset != null ? asset.GetType() : null, asset);
                        return result;
                    }
                }
                if (LoadedAssetOperate_CollectReference(result))
                {
                    return result;
                }

                logger.Trace("Caching failure. assetBundleName={0}, assetName={1}", assetBundleName, assetName);
            }

            AssetLoadingOperation loadingOperation;
            if (loadingOperations.TryGetValue(assetBundleName, out loadingOperation))
            {
                logger.Trace("Asset is loading. assetBundleName={0}, assetName={1}", assetBundleName, assetName);
                return loadingOperation;
            }

            logger.Trace("Start a new loading task. assetBundleName={0}, assetName={1}", assetBundleName, assetName);
            var newLoading = new AssetLoadingOperation
            {
                AssetName = assetName,
                AssetBundleName = assetBundleName,
                AssetType = type
            };
            if (!TryEnqueueLoadingOperation(newLoading))
            {
                return loadingOperationBacklog[assetBundleName];
            }
            CheckStartDownload();
            return newLoading;
        }

        bool TryEnqueueLoadingOperation(AssetLoadingOperation loadingOperation)
        {
            if (!loadingOperationBacklog.ContainsKey(loadingOperation.AssetBundleName))
            {
                loadingOperationBacklog.Add(loadingOperation.AssetBundleName, loadingOperation);
                return true;
            }

            logger.Trace("AssetBundle \"{0}\" is in loading progress.", loadingOperation.AssetBundleName);
            return false;
        }
        
        void FlushLoadingBacklog()
        {
            lock (syncRoot)
            {
                foreach (var pair in loadingOperationBacklog)
                {
                    if (!loadingOperations.ContainsKey(pair.Key))
                    {
                        loadingOperations.Add(pair.Key, pair.Value);
                    }
                    else
                    {
                        logger.Warn("AssetBundle \"{0}\" is in progress.", pair.Key);
                    }
                }
                loadingOperationBacklog.Clear();
            }
        }

        public AssetLoadingOperation LoadSceneAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
            return null;
        }

        public PreloadAssetLoadingOperation PreloadAssets(IEnumerable<string> assetBundleNames)
        {
            // 過濾已經載入好的 assets
            var assetLoadingOperations = new List<AssetLoadingOperation>();
            foreach (string assetBundleName in assetBundleNames)
            {
                var assetLoadingOperation = LoadAssetAsync(assetBundleName);
                if (assetLoadingOperation is FinishedAssetLoadingOperation)
                {
                    continue;
                }
                assetLoadingOperations.Add(assetLoadingOperation);
            }

            // 將過濾後的載入作業添加到預載作業清單
            var preloadAssetLoadingOperations = new PreloadAssetLoadingOperation(assetLoadingOperations);
            if (assetLoadingOperations.Count > 0)
            {
                preloadOperations.Add(preloadAssetLoadingOperations);
            }
            return preloadAssetLoadingOperations;
        }

        public PreloadAssetLoadingOperation PreloadAssets(IEnumerable<PreloadAssetEntry> preloadAssetEntries)
        {
            // 過濾已經載入好的 assets
            var assetLoadingOperations = new List<AssetLoadingOperation>();
            foreach (var preloadAssetEntry in preloadAssetEntries)
            {
                var assetLoadingOperation = LoadAssetAsync(preloadAssetEntry.AssetBundleName, preloadAssetEntry.AssetName);
                if (assetLoadingOperation is FinishedAssetLoadingOperation)
                {
                    continue;
                }
                assetLoadingOperations.Add(assetLoadingOperation);
            }

            // 將過濾後的載入作業添加到預載作業清單
            var preloadAssetLoadingOperations = new PreloadAssetLoadingOperation(assetLoadingOperations);
            if (assetLoadingOperations.Count > 0)
            {
                preloadOperations.Add(preloadAssetLoadingOperations);
            }
            return preloadAssetLoadingOperations;
        }

        void PreloadOperations_OnAssetLoaded()
        {
            foreach (var preloadOperation in preloadOperations)
            {
                if (preloadOperation.IsDone)
                {
                    preloadOperation.OnAssetLoaded();
                }
            }
        }

        public void UnloadAsset(Object assetToUnload)
        {
            if (assetToUnload is GameObject)
            {
#if UNITY_EDITOR
                if (!UnityEditor.AssetDatabase.Contains(assetToUnload))
                {
                    Object.Destroy(assetToUnload);
                }
#else
                Object.Destroy(assetToUnload);
#endif
            }
            else
            {
                Resources.UnloadAsset(assetToUnload);
            }
        }

        public void UnloadUnusedAssets()
        {
            // 即使用 Coroutine 等這個處理完，依然要等數秒，WeakReference 才會被回收
            // 所以就直接呼叫了
            Resources.UnloadUnusedAssets();
            nextClearUnusedAssetTime = Time.realtimeSinceStartup + 5;
        }

        public void UnloadAll()
        {
            // TODO
        }

        // LoadedAssetOperate 物件記錄的 assetbundle 都必須已完成下載
        bool LoadedAssetOperate_CollectReference(FinishedAssetLoadingOperation operation)
        {
            try
            {
                string error;
                var loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(operation.AssetBundleName, out error);
                if (!string.IsNullOrEmpty(error))
                {
                    logger.Error("AssetBundle got some error. assetBundleName={0}, assetName={1}, error={2}",
                                 operation.AssetBundleName, operation.AssetName, error);
                    return false;
                }

                if (loadedAssetBundle == null)
                {
#if UNITY_EDITOR
                    var mainAsset = operation.assetBundleLoadOperation.GetAsset<Object>();
                    if (mainAsset != null && AssetBundleManager.SimulateAssetBundleInEditor)
                    {
                        operation.MainAsset = new WeakReference(mainAsset);
                        return true;
                    }
#endif

                    operation.MainAsset = null;
                    logger.Error("AssetBundle is null. assetBundleName={0}, assetName={1}",
                                 operation.AssetBundleName, operation.AssetName);
                    return false;
                }

                // 第二次讀 assetbundle 耗時幾乎為 0
                // 簡易測試過，實機需要注意情況
                operation.MainAsset = new WeakReference(loadedAssetBundle.m_AssetBundle.LoadAsset(operation.AssetName, operation.AssetType));
                operation.SubAssets = new List<WeakReference>();

                // 排除 DependenciesBundle 內的資源來做無使用判斷
                // 目前以一般 Dependency 的數量不會很多的情況來處理
                var dependenciesBundleName = CollectDependenciesIncludeSubAssetBundles(operation.AssetBundleName);
                var collectLoadedBundle = CollectLoadedAssets(dependenciesBundleName);
                if (operation.MainAsset.Target is GameObject)
                {
                    var go = (operation.MainAsset.Target as GameObject);
                    foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                    {
                        foreach (var material in r.sharedMaterials)
                        {
                            if (material == null)
                            {
                                continue;
                            }

                            if (!collectLoadedBundle.Contains(material.mainTexture))
                            {
                                operation.SubAssets.Add(new WeakReference(material.mainTexture));
                            }
                        }
                    }

                    foreach (var animator in go.GetComponentsInChildren<Animator>(true))
                    {
                        if (!collectLoadedBundle.Contains(animator.runtimeAnimatorController))
                        {
                            operation.SubAssets.Add(new WeakReference(animator.runtimeAnimatorController));
                        }
                    }
                }
                else if (operation.MainAsset.Target is Material)
                {
                    if (!collectLoadedBundle.Contains((operation.MainAsset.Target as Material).mainTexture))
                    {
                        operation.SubAssets.Add(new WeakReference((operation.MainAsset.Target as Material).mainTexture));
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e, "{0}\nLoadedAssetOperate_CollectReference Error", operation.AssetBundleName);
                return false;
            }
        }

        /// <summary>
        /// 取得 asset bundle 自身及其相依性的相依資源。
        /// </summary>
        /// <returns>The dependencies include sub asset bundles.</returns>
        /// <param name="bundleName">Bundle name.</param>
        List<string> CollectDependenciesIncludeSubAssetBundles(string bundleName)
        {
            var dependencies = AssetBundleManager.GetAssetBundleDependencies(bundleName);
            var dependenciesIncludeSubAssetBundles = new List<string>();
            if (dependencies != null && dependencies.Count > 0)
            {
                dependenciesIncludeSubAssetBundles.AddRange(dependencies);
            }
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    var subDependencies = AssetBundleManager.GetAssetBundleDependencies(dependency);
                    if (subDependencies != null && subDependencies.Count > 0)
                    {
                        dependenciesIncludeSubAssetBundles.AddRange(subDependencies);
                    }
                }
            }
            return dependenciesIncludeSubAssetBundles;
        }

        List<Object> CollectLoadedAssets(List<string> assetBundleNames)
        {
            var assets = new List<Object>();
            foreach (var assetBundleName in assetBundleNames)
            {
                string error;
                var assetBundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error);
                if (assetBundle != null && string.IsNullOrEmpty(error))
                {
                    try
                    {
                        // 如果非單一資源assetbundle ，此處可能會做初次讀取，花較長時間
                        foreach (var asset in assetBundle.m_AssetBundle.LoadAllAssets())
                        {
                            if (asset is RuntimeAnimatorController || asset is Texture)
                            {
                                assets.Add(asset);
                            }
                            else if (asset is Material)
                            {
                                assets.Add((asset as Material).mainTexture);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                }
                else
                {
                    logger.Error(error);
                }
            }
            return assets;
        }

        // 以單一 assetbundle 單一 asset 來處理
        void AssetOperateAsync_OnFinished(AssetLoadingOperation assetLoadingOperation)
        {
            logger.Trace("Asset loading completed. assetBundleName={0}, assetName={1}",
                assetLoadingOperation.AssetBundleName, assetLoadingOperation.AssetName);
            assetLoadingOperation.OnAssetLoaded();
            FinishedAssetLoadingOperation loaded;
            if (!loadedAssets.TryGetValue(assetLoadingOperation.AssetBundleName, out loaded))
            {
                loaded = new FinishedAssetLoadingOperation();
                loadedAssets.Add(assetLoadingOperation.AssetBundleName, loaded);
            }
            loaded.AssetName = assetLoadingOperation.AssetName;
            loaded.AssetBundleName = assetLoadingOperation.AssetBundleName;
            loaded.AssetType = assetLoadingOperation.AssetType;
            loaded.assetBundleLoadOperation = assetLoadingOperation.assetBundleLoadOperation;
            LoadedAssetOperate_CollectReference(loaded);
        }

        public bool IsLoaded(string assetBundleName)
        {
            return loadedAssets.ContainsKey(assetBundleName);
        }

        void ClearUnusedAssets()
        {
            if (loadedAssets.Count == 0)
            {
                return;
            }

            // 取得已被釋放的 key(BundleName)，用來釋放 assetbundle 用
            var unusedAssetBundleNames = new List<string>();
            foreach (var pair in loadedAssets)
            {
                var assetBundleName = pair.Key;
                var assetLoadingOperation = pair.Value;

                if (!assetLoadingOperation.IsAlive)
                {
                    unusedAssetBundleNames.Add(assetBundleName);
                }
            }

            // 釋放沒用到的 assets
            foreach (string assetBundleName in unusedAssetBundleNames)
            {
                logger.Debug("Unload AssetBundle {0}", assetBundleName);
                AssetBundleManager.UnloadAssetBundle(assetBundleName);
                loadedAssets.Remove(assetBundleName);
            }
            nextClearUnusedAssetTime = Time.realtimeSinceStartup + 10;
        }

#if UNITY_EDITOR && TESTGUI
        Vector2 scroll;

        void OnGUI()
        {
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(Screen.height * 0.8f), GUILayout.Width(Screen.width * 0.4f));

            foreach (var loaded in _cacheAssets.Values)
            {
                System.Text.StringBuilder content = new System.Text.StringBuilder();
                if (loaded != null)
                {
                    if (loaded.mainAsset != null)
                    {
                        content.AppendLine(string.Format("Main {0} alive {1}", loaded.mainAsset.Target, loaded.mainAsset.IsAlive));
                    }

                    int i = 0;
                    foreach (var sub in loaded.subAssets)
                    {
                        if (sub != null)
                        {
                            content.AppendLine(string.Format("Sub{2} {0} alive {1}", sub.Target, sub.IsAlive, i++));
                        }
                    }
                }
                GUILayout.Box(content.ToString());
            }

            GUILayout.EndScrollView();
        }
#endif
    }
}

// 只支援單一物件 assetbundle，多物件讀取釋放較複雜暫不處理

// 釋放note:
//   用 WeakReference 存物件，在呼叫 Resources.UnloadUnusedAssets() 後會被釋放，已實體化的 GameObject
//   如果再次下載會發生次要資源（animator, texture等）重複載入。
//   保留 AssetBundleManager 的儲存方式避免此問題
//   在呼叫 AssetManager.UnloadUnusedAssets() 後再釋放 AssetBundleManager 中的 AssetBundle
//   釋放判斷需要額外用 WeakReference 存 animator, texture。 material 因為容易新建，無法用來判斷是否真的沒有在用
//

// 此物件記錄的 assetbundle 都必須已完成下載
