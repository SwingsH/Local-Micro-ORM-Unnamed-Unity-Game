using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine.Networking;
using TIZSoft.Utils.Log;

namespace TIZSoft
{
    public class VideoManager : MonoBehaviour
    {
        static readonly Utils.Log.Logger logger = LogManager.Default.FindOrCreateLogger<VideoManager>();

        [SerializeField]
        string fileListName = "file_list.txt";
        public string HostPath;
        [SerializeField]
        string relativePath = "video";
        string localPath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath+"/../", relativePath);
#else
                return Path.Combine(Application.persistentDataPath, relativePath);
#endif
            }
        }
        Dictionary<string,string> filePathList = new Dictionary<string,string>();
        private ReactiveProgress progress ;
        
        private void Awake()
        {
            LoadFileListPath();
        }
        public string GetVideoPath(string clipName)
        {
            var filePath =
                Path.Combine(localPath, clipName);
            if (File.Exists(filePath))
                return filePath;
            else
            {
                if (filePathList.TryGetValue(clipName.ToLower(), out filePath))
                {
                    return Path.Combine(localPath, filePath); ;
                }
                else
                    return string.Empty;
            }
        }
        void LoadFileListPath()
        {
            var fileListPath = Path.Combine(localPath, fileListName);
            if (File.Exists(fileListPath))
            {
                foreach (var path in File.ReadAllLines(fileListPath)
                    .Select(pathSet => pathSet.Split()[0])
                    .Distinct())
                {
                    filePathList[Path.GetFileNameWithoutExtension(path).ToLower()]
                        = path;
                }
            }
        }

        public IObservable<ProgressValueSet> UpdateAllVideo(bool useWWW = true)
        {
            if (progress != null)
                return progress;

            progress = new ReactiveProgress();
            
            if(useWWW)
                StartCoroutine(DownloadVideo_FromWWW());
            else
                StartCoroutine(DownloadVideo_FromWebRequest());
            return progress;
        }

        IEnumerator DownloadVideo_FromWebRequest()
        {
            var fileSourcePath = Path.Combine(HostPath, relativePath);
            var fileListRequest = UnityWebRequest.Get(Path.Combine(fileSourcePath, fileListName));

            logger.Debug("Start Download: {0}", fileListRequest.url);
            yield return fileListRequest.SendWebRequest();
            logger.Debug("Downloaded: {0}, Error: {1}",
                           fileListRequest.url,
                           fileListRequest.error);
            if (!string.IsNullOrEmpty(fileListRequest.error))
            {
                progress.OnCompleted();
                progress = null;
                yield break;
            }
            var sourceFileListText = fileListRequest.downloadHandler.text;
            fileListRequest.Dispose();
            fileListRequest = null;

            Dictionary<string, string> sourceFileList = ParseFileList(sourceFileListText);
            Dictionary<string, string> localFileList;
            var localFileListPath = Path.Combine(localPath, fileListName);
            if (File.Exists(localFileListPath))
            {
                localFileList = ParseFileList(File.ReadAllText(localFileListPath));
            }
            else
                localFileList = new Dictionary<string, string>();

            if(!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            int index = 0;
            progress.TotalAmount = sourceFileList.Count;
            progress.RestAmount = sourceFileList.Count;
            string hash;
            foreach(var fileSet in sourceFileList)
            {
                index++;
                progress.RestAmount--;
                var localFilePath = Path.Combine(localPath, fileSet.Key);
                if (localFileList.TryGetValue(fileSet.Key,out hash))
                {
                    if(hash == fileSet.Value
                        && File.Exists(localFilePath))
                    {
                        continue;
                    }
                }

                var fileRequest = UnityWebRequest.Get(Path.Combine(fileSourcePath, fileSet.Key));

                //todo 加防卡
                logger.Debug("Start Download: {0}", fileRequest.url);
                yield return fileRequest.SendWebRequest();
                logger.Debug("Downloaded: {0}, Error: {1}",
                       fileRequest.url,
                       fileRequest.error);

                if (!string.IsNullOrEmpty(fileRequest.error))
                {
                    progress.ErrorAmount++;
                    continue;
                }
                var fileDirectory = Path.GetDirectoryName(localFilePath);
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                File.WriteAllBytes(localFilePath,fileRequest.downloadHandler.data);
                fileRequest.Dispose();
            }
            File.WriteAllText(localFileListPath, sourceFileListText);
            LoadFileListPath();

            logger.Debug("Update From WebRequest Completed");

            progress.OnCompleted();
            progress = null;
        }

        IEnumerator DownloadVideo_FromWWW()
        {
            var fileSourcePath = Path.Combine(HostPath, relativePath);
            var fileListRequest = new WWW(Path.Combine(fileSourcePath, fileListName));

            logger.Debug("Start Download: {0}", fileListRequest.url);
            yield return fileListRequest;
            logger.Debug("Downloaded: {0}, Error: {1}",
                           fileListRequest.url,
                           string.IsNullOrEmpty(fileListRequest.error)
                            ? "None": fileListRequest.error);
            if (!string.IsNullOrEmpty(fileListRequest.error))
            {
                logger.Debug("Update From WWW Completed");
                progress.OnCompleted();
                progress = null;
                yield break;
            }
            var sourceFileListText = fileListRequest.text;
            fileListRequest.Dispose();
            fileListRequest = null;

            Dictionary<string, string> sourceFileList = ParseFileList(sourceFileListText);
            Dictionary<string, string> localFileList;
            var localFileListPath = Path.Combine(localPath, fileListName);
            if (File.Exists(localFileListPath))
            {
                localFileList = ParseFileList(File.ReadAllText(localFileListPath));
            }
            else
                localFileList = new Dictionary<string, string>();

            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            int index = 0;
            progress.TotalAmount = sourceFileList.Count;
            progress.RestAmount = sourceFileList.Count;
            string hash;
            foreach (var fileSet in sourceFileList)
            {
                index++;
                progress.RestAmount--;

                if (localFileList.TryGetValue(fileSet.Key, out hash))
                {
                    if (hash == fileSet.Value)
                    {
                        logger.Debug("Is Up To Date: {0}", fileSet.Key);
                        continue;
                    }
                }

                var fileRequest = new WWW(Path.Combine(fileSourcePath, fileSet.Key));

                //todo 加防卡
                logger.Debug("Start Download: {0}", fileRequest.url);
                yield return fileRequest;
                logger.Debug("Downloaded: {0}, Error: {1}",
                       fileRequest.url,
                       string.IsNullOrEmpty(fileRequest.error)
                            ? "None" : fileRequest.error);

                if (!string.IsNullOrEmpty(fileRequest.error))
                {
                    progress.ErrorAmount++;
                    continue;
                }
                var localFilePath = Path.Combine(localPath, fileSet.Key);
                var fileDirectory = Path.GetDirectoryName(localFilePath);
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);
                File.WriteAllBytes(localFilePath, fileRequest.bytes);
                fileRequest.Dispose();
            }
            File.WriteAllText(localFileListPath, sourceFileListText);
            LoadFileListPath();
            logger.Debug("Update From WWW Completed");
            progress.OnCompleted();
            progress = null;
        }

        Dictionary<string,string> ParseFileList(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Dictionary<string, string>();
            var dict = new Dictionary<string, string>();

            var reader = new StringReader(text);
            var line = string.Empty;
            while(!string.IsNullOrEmpty( line = reader.ReadLine()))
            {
                var values = line.Split();
                if(values.Length > 1)
                {
                    dict[values[0]] = values[1];
                }
            }
            return dict;
        }

        private void OnDestroy()
        {
            if(progress != null)
                progress.OnCompleted();
        }
    }
}
