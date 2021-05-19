#define Q9997 // Nick 20180608

using AssetBundles;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tizsoft.AssetManagement
{
    public class PreloadAssetEntry
    {
        public string AssetBundleName { get; private set; }

        public string AssetName { get; private set; }

        public PreloadAssetEntry(string assetBundleName, string assetName)
        {
            AssetBundleName = assetBundleName;
            AssetName = assetName;
        }
    }

    public enum AssetBundleSourceType
    {
        StreamingAssets,
        Remote
    }

    /// <summary>
    /// 表示一個 AssetManager 的設定資料類別。
    /// </summary>
    [Serializable]
    public class AssetManagerSettings
    {
        [SerializeField]
        [Tooltip("取得或設定是否要自動初始化。若設為 false，則需要自行呼叫 Initialize")]
        bool shouldInitializeAutomatically;

        [SerializeField]
        [Tooltip("設定 AssetBundleManager 是否要使用模擬模式。若為 true，則其他下載設定均會被此設定取代。此設定只在 Editor 下有用。")]
        bool isSimulationMode;

        [SerializeField]
        [Tooltip("設定是否使用開發版 AssetBundle 伺服器。若為 true，則 SourceType 的設定將會被此設定取代。")]
        bool isDevelopmentAssetBundleServer;

        [SerializeField]
        [Tooltip("設定 AssetBundle 檔案來源。若 IsDevelopmentAssetBundleServer 為 true，則此設定無效。")]
        AssetBundleSourceType assetBundleSourceType = AssetBundleSourceType.Remote;

        [SerializeField]
        [Tooltip("設定 AssetBundle 檔案來源的 URL。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 Remote 才有效。")]
        string baseDownloadingUrl = "http://localhost:7888";

        [SerializeField]
        [Tooltip("設定 AssetBundle 檔案來源的相對路徑。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 StreamingAssets 才有效。")]
        string relativePath = Utility.AssetBundlesOutputPath;

        [SerializeField]
        [Tooltip("AssetBundle manifest 檔名。不填的話，則預設為目標平台名稱。")]
        string manifestAssetBundleName;

        [SerializeField]
        [Tooltip("Log mode.")]
        AssetBundleManager.LogMode logMode = AssetBundleManager.LogMode.JustErrors;

        [SerializeField]
        int maxUpdateFileAmount = 5;

        [SerializeField]
        bool provideAtlas = false;

        [SerializeField]
        string atlasPath = "atlas";

        [SerializeField]
        string scenePath = "scene";

        /// <summary>
        /// 取得或設定是否要自動初始化。若設為 false，則需要自行呼叫 <see cref="IAssetManager.Initialize()"/>。
        /// </summary>
        public bool ShouldInitializeAutomatically
        {
            get { return shouldInitializeAutomatically; }
            set { shouldInitializeAutomatically = value; }
        }

        /// <summary>
        /// 設定 AssetBundleManager 是否要使用模擬模式。若為 true，則其他下載設定均會被此設定取代。此設定只在 Editor 下有用。
        /// </summary>
        public bool IsSimulationMode
        {
            get { return isSimulationMode; }
            set { isSimulationMode = value; }
        }

        /// <summary>
        /// 設定是否使用開發版 AssetBundle 伺服器。若為 true，則 SourceType 的設定將會被此設定取代。
        /// </summary>
        public bool IsDevelopmentAssetBundleServer
        {
            get { return isDevelopmentAssetBundleServer; }
            set { isDevelopmentAssetBundleServer = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源。若 IsDevelopmentAssetBundleServer 為 true，則此設定無效。
        /// </summary>
        public AssetBundleSourceType AssetBundleSourceType
        {
            get { return assetBundleSourceType; }
            set { assetBundleSourceType = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源的 URL。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 Remote 才有效。
        /// </summary>
        public string BaseDownloadingUrl
        {
            get { return baseDownloadingUrl; }
            set { baseDownloadingUrl = value; }
        }

        /// <summary>
        /// 設定 AssetBundle 檔案來源的相對路徑。IsDevelopmentAssetBundleServer 為 false 且 SourceType 為 StreamingAssets 才有效。
        /// </summary>
        public string RelativePath
        {
            get { return relativePath; }
            set { relativePath = value; }
        }

        /// <summary>
        /// AssetBundle manifest 檔名。不填的話，則預設為目標平台名稱。
        /// </summary>
        public string ManifestAssetBundleName
        {
            get { return manifestAssetBundleName; }
            set { manifestAssetBundleName = value; }
        }

        /// <summary>
        /// AssetBundleManager 的 LogMode，預設為 <see cref="AssetBundleManager.LogMode.JustErrors"/>。
        /// </summary>
        public AssetBundleManager.LogMode LogMode
        {
            get { return logMode; }
            set { logMode = value; }
        }

        public int MaxUpdateFileAmount
        {
            get { return maxUpdateFileAmount; }
            set { maxUpdateFileAmount = value; }
        }

        public bool ProvideAtlas
        {
            get { return provideAtlas; }
            set { provideAtlas = value; }
        }

        public string AtlasPath
        {
            get { return atlasPath; }
            set { atlasPath = value; }
        }

        public string ScenePath
        {
            get { return scenePath; }
            set { scenePath = value; }
        }
    }
    
    /// <summary>
    /// 表示一個遊戲資源管理器。
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// 取得是否已經初始化完畢。
        /// </summary>
        IReadOnlyReactiveProperty<bool> IsInitialized { get; }

        /// <summary>
        /// 取得或設定資源管理設定。
        /// </summary>
        AssetManagerSettings Settings { get; set; }

        /// <summary>
        /// 進行初始化作業，初始化完成後才能使用載入方法。
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 以非同步的方式載入一個 Asset。
        /// </summary>
        /// <param name="assetBundleName">完整 assetbundle 相對路徑</param>
        /// <returns></returns>
        AssetLoadingOperation LoadAssetAsync(string assetBundleName);

        /// <summary>
        /// 以非同步的方式載入一個 Asset。
        /// </summary>
        /// <param name="assetBundleName">完整 assetbundle 相對路徑</param>
        /// <param name="assetName">assetName，空值自動取 assetbundle Path.FileName</param>
        /// <returns></returns>
        AssetLoadingOperation LoadAssetAsync(string assetBundleName, string assetName);

        /// <summary>
        /// 以非同步的方式載入一個 Asset。
        /// </summary>
        /// <param name="assetBundleName">完整 assetbundle 相對路徑</param>
        /// <param name="assetName">assetName，空值自動取 assetbundle Path.FileName</param>
        /// <param name="type"></param>
        /// <returns></returns>
        AssetLoadingOperation LoadAssetAsync(string assetBundleName, string assetName, Type type);

        AssetLoadingOperation LoadSceneAsync(string assetBundleName, string sceneName, bool isAdditive);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetBundleNames">完整 assetbundle 相對路徑，assetName 取 assetbundle Path.FileName </param>
        PreloadAssetLoadingOperation PreloadAssets(IEnumerable<string> assetBundleNames);

        PreloadAssetLoadingOperation PreloadAssets(IEnumerable<PreloadAssetEntry> preloadAssetEntries);

        void UnloadAsset(Object assetToUnload);
        void UnloadUnusedAssets();
        void UnloadAll();

#if Q9997 //  Nick 20180608

        void UnloadAssetBundle(string sAssetBundleName);
#endif //Q9997

        IObservable<ProgressValueSet> UpdateAllAsset();
    }
}

// 只支援單一物件assetbundle，多物件讀取釋放較複雜暫不處理
