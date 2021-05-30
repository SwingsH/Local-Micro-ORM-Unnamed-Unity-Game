using System;
using System.Collections;
using System.Collections.Generic;
using TIZSoft.Extensions;
using TIZSoft.Utils.Log;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Logger = TIZSoft.Utils.Log.Logger;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TIZSoft
{
    /// <summary>
    /// 表示一個場景管理器，提供場景存取操作、事件調用等。
    /// </summary>
    public class SceneManager
    {
        class ProgressReporter : UniRx.IProgress<float>
        {
            readonly string actionName;
            readonly string sceneName;

            public ProgressReporter(string actionName, string sceneName)
            {
                this.actionName = actionName;
                this.sceneName = sceneName;
            }

            public void Report(float value)
            {
                logger.Info("Scene {0} {1} progress: {2:P2}", sceneName, actionName, value);
            }
        }

        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<SceneManager>();
        
        readonly Dictionary<string, List<Action<Scene, LoadSceneMode, string>>> onDoneCallbackMap =
            new Dictionary<string, List<Action<Scene, LoadSceneMode, string>>>();
        Scene? lastLoadedScene;
        LoadSceneMode lastLoadSceneMode;
        string relatePath;
        AssetBundles.AssetBundleManager assetBundleManager;

        HashSet<string> sceneInBuild = new HashSet<string>();

        public SceneManager()
        {
            UnitySceneManager.sceneLoaded += OnSceneLoaded;
            Observable.FromMicroCoroutine(Tick).Subscribe();
        }

        public SceneManager(string relatePath, AssetBundles.AssetBundleManager assetBundleManager)
            : this()
        {
            this.relatePath = relatePath;
            this.assetBundleManager = assetBundleManager;
            
            for (int i = 0; i<UnitySceneManager.sceneCountInBuildSettings;i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                sceneInBuild.Add(System.IO.Path.GetFileNameWithoutExtension(scenePath));
            }
        }

        /// <summary>
        /// 切換到指定場境，這將會卸載現有的場景。
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(string sceneName)
        {
            ChangeScene(sceneName, null);
        }

        /// <summary>
        /// 切換到指定場境，這將會卸載現有的場景。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="onDone">action Param: Scene, LoadSceneMode, error String</param>
        public void ChangeScene(string sceneName, Action<Scene, LoadSceneMode, string> onDone)
        {
            LoadSceneCore(sceneName, LoadSceneMode.Single, onDone);
        }

        /// <summary>
        /// 以 Additive 的方式加載場景。
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneAdditive(string sceneName)
        {
            LoadSceneAdditive(sceneName, null);
        }

        /// <summary>
        /// 以 Additive 的方式加載場景。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="onDone">action Param: Scene, LoadSceneMode, error String</param>
        public void LoadSceneAdditive(string sceneName, Action<Scene, LoadSceneMode, string> onDone)
        {
            LoadSceneCore(sceneName, LoadSceneMode.Additive, onDone);
        }

        void LoadSceneCore(string sceneName, LoadSceneMode loadSceneMode, Action<Scene, LoadSceneMode, string> onDone)
        {
            if (onDone != null)
            {
                List<Action<Scene, LoadSceneMode, string>> onDoneCallbacks;
                if (onDoneCallbackMap.ContainsKey(sceneName))
                {
                    onDoneCallbacks = onDoneCallbackMap[sceneName];
                }
                else
                {
                    onDoneCallbacks = new List<Action<Scene, LoadSceneMode, string>>();
                    onDoneCallbackMap.Add(sceneName, onDoneCallbacks);
                }
                onDoneCallbacks.Add(onDone);
            }

            if (assetBundleManager == null
                || sceneInBuild.Contains(sceneName))
            {
                UnitySceneManager.LoadSceneAsync(sceneName, loadSceneMode)
                    .AsObservable(new ProgressReporter("loading", sceneName))
                    .DoOnError(logger.Error)
                    .Subscribe();
            }
            else
            {
                var scenePath = string.Join("/", new string[] { relatePath, sceneName });

                scenePath = scenePath.ToLower();

                assetBundleManager.LoadLevelAsync(
                    scenePath,
                    sceneName,
                    loadSceneMode == LoadSceneMode.Additive).ToObservable()
                    .DoOnCompleted(() =>
                        {
                            string error;
                            assetBundleManager.GetLoadedAssetBundle(scenePath, out error);
                            if(!string.IsNullOrEmpty(error))
                                logger.Error(error);
                            SceneLoadedCallback(sceneName, loadSceneMode, error);

                            Resources.UnloadUnusedAssets();
                            GC.Collect();
                        })
                    .Subscribe();
            }
        }

        void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            lastLoadedScene = loadedScene;
            lastLoadSceneMode = loadSceneMode;
        }
        
        IEnumerator Tick()
        {
            while (true)
            {
                yield return null;

                if (lastLoadedScene == null)
                {
                    continue;
                }

                var loadedScene = lastLoadedScene.Value;
                while (!loadedScene.isLoaded)
                {
                    yield return null;
                }
                yield return null;

                SceneLoadedCallback(loadedScene.name, lastLoadSceneMode);
                
                lastLoadedScene = null;
            }
        }

        void SceneLoadedCallback(string sceneName, LoadSceneMode mode, string error = "")
        {
            var loadedScene = UnitySceneManager.GetSceneByName(sceneName);
            List<Action<Scene, LoadSceneMode, string>> onDoneCallbacks;
            if (onDoneCallbackMap.TryGetValue(sceneName, out onDoneCallbacks) &&
                onDoneCallbacks.Count > 0)
            {
                foreach (var onDone in onDoneCallbacks)
                {
                    onDone(loadedScene, mode, string.Empty);
                }
                onDoneCallbacks.Clear();
            }
        }

        /// <summary>
        /// 卸載指定場景。
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnloadScene(string sceneName)
        {
            UnloadScene(sceneName, null);

            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 卸載指定場景。
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="onDone"></param>
        public void UnloadScene(string sceneName, Action<Scene> onDone)
        {
            UnloadSceneCore(sceneName, onDone);
        }

        void UnloadSceneCore(string sceneName, Action<Scene> onDone)
        {
            UnityAction<Scene> onSceneUnloaded = null;
            onSceneUnloaded = unloadedScene =>
            {
                UnitySceneManager.sceneUnloaded -= onSceneUnloaded;
                onDone.Raise(unloadedScene);
            };
            UnitySceneManager.sceneUnloaded += onSceneUnloaded;
            UnitySceneManager.UnloadSceneAsync(sceneName)
                .AsObservable(new ProgressReporter("unloading", sceneName))
                .DoOnError(logger.Error)
                .Subscribe();
        }
    }
}
