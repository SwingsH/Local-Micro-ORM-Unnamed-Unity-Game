using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIZSoft.Log;
using TIZSoft.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace TIZSoft
{
    /// <summary>
    /// 表示一個 Prefab 載入元件。
    /// </summary>
    /// <remarks>
    /// 此元件需要在使用前呼叫 <see cref="AddAssetProvider"/> 來設定 <see cref="AssetProvider"/> 才能使用。
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("TIZSoft/Prefab Loader")]
    public class PrefabLoader : MonoBehaviour
    {
        /// <summary>
        /// 定義 PrefabLoader 載入時機。
        /// </summary>
        public enum LoadingTiming
        {
            /// <summary>
            /// 永不自動載入，需自行呼叫 Load() 方法。
            /// </summary>
            Never = 0,

            /// <summary>
            /// 於 MonoBehaviour Awake() 時載入。
            /// </summary>
            Awake = 1,

            /// <summary>
            /// 於 MonoBehaviour Start() 時載入。
            /// </summary>
            Start = 2
        }

        /// <summary>
        /// 定義 prefab 載入方法。
        /// </summary>
        public enum PrefabSourceType
        {
            /// <summary>
            /// 來源為從 inspector 上直接設定的 prefab。
            /// </summary>
            FromPrefab = 0,

            /// <summary>
            /// 來源為 AssetBundle。
            /// </summary>
            FromAssetBundle = 1
        }

        /// <summary>
        /// 定義載入狀態。
        /// </summary>
        public enum LoadingState
        {
            /// <summary>
            /// 尚未載入 prefab。
            /// </summary>
            Unloaded,

            /// <summary>
            /// 正在載入 prefab
            /// </summary>
            Loading,

            /// <summary>
            /// Prefab 已載入完畢。
            /// </summary>
            Loaded,

            /// <summary>
            /// 載入發生錯誤。
            /// </summary>
            ErrorOccurred
        }

        [Serializable]
        public class LoadCompletedEvent : UnityEvent<PrefabLoader> { }

        static readonly Log.Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();

        /// <summary>
        /// 方便存取操作 AssetProvider。
        /// </summary>
        static readonly Dictionary<string, AssetProvider> AssetProviders = new Dictionary<string, AssetProvider>();

        [SerializeField]
        [Tooltip("Prefab 載入時機。")]
        LoadingTiming timing;

        [SerializeField]
        [Tooltip("Prefab 來源。")]
        PrefabSourceType sourceType;

        [SerializeField]
        [Tooltip("設定 instantiate 後是否要保持原本的世界座標位置")]
        bool worldPositionStays = true;

        [SerializeField]
        [Tooltip("實際上使用的 prefab。注意！此欄位不可以是在場景上的物件 (意即 prefab instance)。")]
        GameObject prefab;

        [SerializeField]
        [Tooltip("設定要使用哪一個 AssetProvider 來操作。這個名稱通常和 AssetProvider 使用的 AssetSettings.name 一樣。")]
        string assetProviderName;

        [SerializeField]
        [Tooltip("要載入的 AssetBundle Category。")]
        string category;

        [SerializeField]
        [Tooltip("要從 AssetBundle 載入的 asset 名稱。")]
        string assetName;

        [Header("Arguments")]
        [SerializeField]
        [Tooltip("自訂整數參數，可用於載入完成事件需要提供的額外參數值。")]
        int intValue;
        
        readonly ReactiveProperty<GameObject> instance = new ReactiveProperty<GameObject>();

        [SerializeField]
        LoadCompletedEvent onLoadCompleted = new LoadCompletedEvent();

        Loader loader;

        /// <summary>
        /// 取得或設定 prefab 載入時機。
        /// </summary>
        /// <value>The prefab loading timing.</value>
        public LoadingTiming Timing
        {
            get { return timing; }
            set { timing = value; }
        }

        /// <summary>
        /// Prefab 來源。
        /// </summary>
        public PrefabSourceType SourceType
        {
            get { return sourceType; }
            set { sourceType = value; }
        }

        /// <summary>
        /// 取得或設定實際上使用的 prefab。
        /// 注意！此欄位不可以是在場景上的物件 (意即 prefab instance)。
        /// 請參考 UnityEditor 裡的 PrefabUtility.GetPrefabType()。
        /// </summary>
        /// <value>The prefab.</value>
        public GameObject Prefab
        {
            get { return prefab; }
            set { prefab = value; }
        }

        /// <summary>
        /// 取得或設定 instantiate 後是否要保持原本的世界座標位置。
        /// </summary>
        public bool WorldPositionStays
        {
            get { return worldPositionStays; }
            set { worldPositionStays = value; }
        }

        /// <summary>
        /// 設定要使用哪一個 AssetProvider 來操作。這個名稱通常和 AssetProvider 使用的 AssetSettings.name 一樣。
        /// </summary>
        public string AssetProviderName
        {
            get { return assetProviderName; }
            set { assetProviderName = value; }
        }

        /// <summary>
        /// 要載入的 AssetBundle。
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// 要從 AssetBundle 載入的 asset 名稱。
        /// </summary>
        public string AssetName
        {
            get { return assetName; }
            set { assetName = value; }
        }

        /// <summary>
        /// 自訂整數參數，可用於載入完成事件需要提供的額外參數值。
        /// </summary>
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        /// <summary>
        /// 取得 instance。
        /// </summary>
        public IReadOnlyReactiveProperty<GameObject> Instance
        {
            get { return instance.ToReadOnlyReactiveProperty(); }
        }

        /// <summary>
        /// 判斷是否已載入。
        /// </summary>
        public bool IsLoaded
        {
            get { return State == LoadingState.Loaded; }
        }

        /// <summary>
        /// 取得載入狀態。
        /// </summary>
        public LoadingState State { get; private set; }

        /// <summary>
        /// 添加一個 AssetProvider 到此全域空間供所有 PrefabLoader 使用。
        /// </summary>
        /// <param name="assetProvider"></param>
        public static void AddAssetProvider(AssetProvider assetProvider)
        {
            ExceptionUtils.VerifyArgumentNull(assetProvider, "assetProvider");
            ExceptionUtils.VerifyArgumentNullOrEmpty(assetProvider.Name, "assetProvider.Name");

            if (AssetProviders.ContainsKey(assetProvider.Name))
            {
                logger.Error("Name of AssetProvider {0} already exists.", assetProvider.Name);
                return;
            }

            AssetProviders[assetProvider.Name] = assetProvider;
        }

        /// <summary>
        /// 從全域空間移除指定的 AssetProvider。
        /// </summary>
        /// <param name="assetProviderName"></param>
        /// <returns></returns>
        public static bool RemoveAssetProvider(string assetProviderName)
        {
            ExceptionUtils.VerifyArgumentNullOrEmpty(assetProviderName, "assetProviderName");

            return AssetProviders.Remove(assetProviderName);
        }

        static AssetProvider FindAssetProvider(string assetProviderName)
        {
            ExceptionUtils.VerifyArgumentNullOrEmpty(assetProviderName, "assetProviderName");

            AssetProvider assetProvider;
            if (AssetProviders.TryGetValue(assetProviderName, out assetProvider))
            {
                return assetProvider;
            }

            logger.Error("Could not found {0}.", assetProviderName);
            return null;
        }

        void Awake()
        {
            LoadIfTimingMatched(LoadingTiming.Awake);
        }

        void Start()
        {
            LoadIfTimingMatched(LoadingTiming.Start);
        }

        void LoadIfTimingMatched(LoadingTiming timing)
        {
            if (Timing == timing)
            {
                LoadAsync().Subscribe();
            }
        }

        /// <summary>
        /// 載入 prefabs。
        /// </summary>
        [ContextMenu("Load")]
        public void Load()
        {
            LoadAsync().Subscribe();
        }

        /// <summary>
        /// 載入 prefabs。
        /// </summary>
        public IObservable<GameObject> LoadAsync()
        {
            return LoadAsyncCore();
        }

        IObservable<GameObject> LoadAsyncCore()
        {
            if (State >= LoadingState.Loading && State < LoadingState.ErrorOccurred)
            {
                return State == LoadingState.Loaded ? Instance.AsObservable() : loader;
            }

            State = LoadingState.Loading;
            loader = new Loader(this);
            return loader;
        }

        /// <summary>
        /// 卸載 prefabs。
        /// </summary>
        [ContextMenu("Unload")]
        public void Unload()
        {
            UnloadCore();
        }

        void UnloadCore()
        {
            // 卸載 prefabs
            if (Instance.HasValue && Instance.Value != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(Instance.Value);
                }
                else
                {
                    DestroyImmediate(Instance.Value);
                }
            }

            // 保證清除所有的子物件
            foreach (Transform child in transform)
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            loader = null;
            State = LoadingState.Unloaded;
        }

        class Loader : IObservable<GameObject>
        {
            readonly PrefabLoader parent;

            IObserver<GameObject> outObserver;

            public Loader(PrefabLoader parent)
            {
                this.parent = parent;
            }

            public IDisposable Subscribe(IObserver<GameObject> observer)
            {
                outObserver = observer;

                // 如果有子物件，先清除乾淨
                parent.Unload();

                switch (parent.SourceType)
                {
                    case PrefabSourceType.FromPrefab:
                        OnAssetLoaded(parent.Prefab);
                        break;
                    case PrefabSourceType.FromAssetBundle:
                        // 讓 Load 也能在 non-Play mode Editor 下運作
                        if (Application.isPlaying)
                        {
                            var assetProvider = FindAssetProvider(parent.AssetProviderName);
                            if (assetProvider != null)
                            {
                                assetProvider.LoadAsync<GameObject>(parent.Category, parent.AssetName, OnAssetLoaded);
                            }
                            else
                            {
                                Error(observer, "AssetProvider not exists.");
                            }
                        }
#if UNITY_EDITOR
                        else if (Application.isEditor)
                        {
                            // 模擬載入
                            var candidates = UnityEditor.AssetDatabase.FindAssets("t:AssetSettings");
                            foreach (var assetSettings in candidates
                                                        .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                                                        .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AssetSettings>)
                                                        .Where(assetSettings => assetSettings.name == parent.AssetProviderName))
                            {
                                if (assetSettings.Entries.Count(e => e.Category == parent.Category) == 0)
                                {
                                    Error(observer, "Category not found.");
                                    break;
                                }

                                foreach (var assetPaths in from e in assetSettings.Entries
                                                           where e.Category == parent.Category
                                                           select e.GetAssetBundleName(parent.AssetName) into assetBundleName
                                                           select UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, parent.AssetName))
                                {
                                    if (assetPaths.Length > 0)
                                    {
                                        var asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                                        if (asset == null)
                                        {
                                            Error(observer, "Asset is null.");
                                            break;
                                        }

                                        var go = asset as GameObject;
                                        if (go == null)
                                        {
                                            Error(observer, "Expected asset type is GameObject, but was {3}.", asset.GetType());
                                            break;
                                        }

                                        OnAssetLoaded(go);
                                    }
                                    else
                                    {
                                        Error(observer, "Asset not found.");
                                    }
                                    break;
                                }
                                break;
                            }
                        }
#endif
                        else
                        {
                            Error(observer, "Unexpected application playing state. Asset loading operation has been suspended.");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                // AssetProvider 沒有提供中斷的方法，所以沒辦法 unsubscribe。
                return Disposable.Empty;
            }

            void OnAssetLoaded(GameObject prefab)
            {
                if (prefab != null)
                {
                    parent.Prefab = prefab;
                    if (parent.Prefab != null)
                    {
                        Exception lastException = null;

                        // Instantiate 失敗也算是載入失敗，因此要確保在出錯時是呼叫 OnError()，
                        // 而不是 OnNext()，避免傳遞錯誤的物件至 subscriber。
                        try
                        {
                            InstantiatePrefab();
                        }
                        catch (Exception e)
                        {
                            lastException = e;
                            Error(outObserver, e.ToString());
                        }

                        if (lastException == null)
                        {
                            parent.State = LoadingState.Loaded;
                            outObserver.OnNext(parent.instance.Value);
                            outObserver.OnCompleted();
                        }
                    }
                    else
                    {
                        Error(outObserver, "Asset is not a GameObject.");
                    }
                }
                else
                {
                    Error(outObserver);
                }

                parent.onLoadCompleted.Invoke(parent);

                // 編輯模式下不需要 disable
                if (Application.isPlaying)
                {
                    parent.enabled = false;
                }

                parent.loader = null;
            }

            void InstantiatePrefab()
            {
                if (Application.isPlaying)
                {
                    var instance = Instantiate(parent.Prefab, parent.transform, parent.WorldPositionStays);

                    // 把 "(Clone)" 去掉
                    instance.name = instance.name.Substring(0, instance.name.Length - "(Clone)".Length);
                    parent.instance.Value = instance;
                }
#if UNITY_EDITOR
                else
                {
                    // Editor 編輯模式下改用 prefab 工具 instantiate prefab (藍色字樣)
                    var instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(parent.Prefab);
                    instance.transform.SetParent(parent.transform, parent.WorldPositionStays);
                    parent.instance.Value = instance;
                }
#endif
            }

            void Error(IObserver<GameObject> observer, string format = null, params object[] args)
            {
                parent.loader = null;
                parent.State = LoadingState.ErrorOccurred;

                var message = new StringBuilder()
                    .AppendFormat("Couldn't load prefab \"{0}\" from category \"{1}\" in \"{2}\" AssetSettings.\n",
                        parent.AssetName, parent.Category, parent.AssetProviderName);
                if (!string.IsNullOrEmpty(format))
                {
                    if (args == null)
                    {
                        message.AppendLine(format);
                    }
                    else
                    {
                        message.AppendFormat(format, args);
                        message.AppendLine();
                    }
                }
                observer.OnError(new Exception(message.ToString()));
            }
        }
    }
}
