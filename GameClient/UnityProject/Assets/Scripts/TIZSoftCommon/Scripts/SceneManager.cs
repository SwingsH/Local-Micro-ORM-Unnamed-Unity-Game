using System;
using System.Collections;
using System.Collections.Generic;
using TIZSoft.Extensions;
using TIZSoft.Log;
using UniRx;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Logger = TIZSoft.Log.Logger;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TIZSoft
{
    /// <summary>
    /// 表示一個場景管理器，提供場景存取操作、事件調用等。
    /// </summary>
    public class SceneManager
    {
        class ProgressReporter : IProgress<float>
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
        
        readonly Dictionary<string, List<Action<Scene, LoadSceneMode>>> onDoneCallbackMap =
            new Dictionary<string, List<Action<Scene, LoadSceneMode>>>();
        Scene? lastLoadedScene;
        LoadSceneMode lastLoadSceneMode;

        public SceneManager()
        {
            UnitySceneManager.sceneLoaded += OnSceneLoaded;
            Observable.FromMicroCoroutine(Tick).Subscribe();
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
        /// <param name="onDone"></param>
        public void ChangeScene(string sceneName, Action<Scene, LoadSceneMode> onDone)
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
        /// <param name="onDone"></param>
        public void LoadSceneAdditive(string sceneName, Action<Scene, LoadSceneMode> onDone)
        {
            LoadSceneCore(sceneName, LoadSceneMode.Additive, onDone);
        }

        void LoadSceneCore(string sceneName, LoadSceneMode loadSceneMode, Action<Scene, LoadSceneMode> onDone)
        {
            if (onDone != null)
            {
                List<Action<Scene, LoadSceneMode>> onDoneCallbacks;
                if (onDoneCallbackMap.ContainsKey(sceneName))
                {
                    onDoneCallbacks = onDoneCallbackMap[sceneName];
                }
                else
                {
                    onDoneCallbacks = new List<Action<Scene, LoadSceneMode>>();
                    onDoneCallbackMap.Add(sceneName, onDoneCallbacks);
                }
                onDoneCallbacks.Add(onDone);
            }

            UnitySceneManager.LoadSceneAsync(sceneName, loadSceneMode)
                .AsObservable(new ProgressReporter("loading", sceneName))
                .DoOnError(logger.Error)
                .Subscribe();
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

                List<Action<Scene, LoadSceneMode>> onDoneCallbacks;
                if (onDoneCallbackMap.TryGetValue(loadedScene.name, out onDoneCallbacks) &&
                    onDoneCallbacks.Count > 0)
                {
                    foreach (var onDone in onDoneCallbacks)
                    {
                        onDone(loadedScene, lastLoadSceneMode);
                    }
                    onDoneCallbacks.Clear();
                }
                lastLoadedScene = null;
            }
        }

        /// <summary>
        /// 卸載指定場景。
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnloadScene(string sceneName)
        {
            UnloadScene(sceneName, null);
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
