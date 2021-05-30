using System;
using System.Collections;
using System.Collections.Generic;
using TIZSoft.AssetManagement;
using TIZSoft.Extensions;
using TIZSoft.Utils.Log;
using UniRx;
using Object = UnityEngine.Object;

namespace TIZSoft
{
    /// <summary>
    /// 表示一個 high-level asset 供應者。
    /// </summary>
    public class AssetProvider
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<AssetProvider>();

        readonly Dictionary<string, AssetSettings.Entry> cachedSettings = new Dictionary<string, AssetSettings.Entry>();

        readonly IAssetManager assetManager;

        /// <summary>
        /// 當這個 AssetProvider 抓不到東西時，將會嘗試去 fallback 抓。
        /// </summary>
        readonly HashSet<AssetProvider> fallbackAssetProviders = new HashSet<AssetProvider>();

        /// <summary>
        /// 取得 AssetProvider 的名稱，與 AssetSettings.name 一致。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 取得是否已初始化完畢。
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsInitialized
        {
            get { return assetManager.IsInitialized; }
        }

        public AssetProvider(AssetSettings settings, IAssetManager assetManager, params AssetProvider[] fallbackAssetProviders)
        {
            Name = settings.name;
            this.assetManager = assetManager;
            foreach (var fallbackAssetProvider in fallbackAssetProviders)
            {
                if (fallbackAssetProvider != null && fallbackAssetProvider != this)
                {
                    this.fallbackAssetProviders.Add(fallbackAssetProvider);
                }
            }
            BuildSettingsCache(settings);
        }
        
        void BuildSettingsCache(AssetSettings settings)
        {
            // 先 cache 並 normalize
            var index = 0;
            foreach (var entry in settings.Entries)
            {
                if (entry == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(entry.Category))
                {
                    logger.Error("Entries[{0}].Category is null or empty.", index);
                    continue;
                }

                if (cachedSettings.ContainsKey(entry.Category))
                {
                    logger.Error("Category {0} is already registered!", entry.Category);
                    continue;
                }

                NormalizeEntry(entry);
                cachedSettings.Add(entry.Category, entry);
                ++index;
            }
        }

        public void Initialize()
        {
            // Do nothing.
        }

        static void NormalizeEntry(AssetSettings.Entry entry)
        {
            // TODO: 或許有用更有效的方式正規化
            entry.DirectoryName = string.Concat(entry.DirectoryName.Trim('/'), "/");
        }

        /// <summary>
        /// 非同步載入一個特定型別的 asset。
        /// </summary>
        /// <param name="category">Category.</param>
        /// <param name="assetName">Asset name.</param>
        /// <param name="onDone">On done.</param>
        /// <typeparam name="T">The type derived from UnityEngine.Object.</typeparam>
        public void LoadAsync<T>(string category, string assetName, Action<T> onDone)
            where T : Object
        {
            LoadAsync(category, string.Empty, assetName,  onDone);
        }
        /// <summary>
        /// 非同步載入一個特定型別的 asset。
        /// </summary>
        /// <param name="category">Category.</param>
        /// <param name="assetName">Asset name.</param>
        /// <param name="assetBundleName">Asset Bundle name.</param>
        /// <param name="onDone">On done.</param>
        /// <typeparam name="T">The type derived from UnityEngine.Object.</typeparam>
        public void LoadAsync<T>(string category, string assetBundleName, string assetName, Action<T> onDone)
            where T : Object
        {
            LoadAsync(category, assetBundleName, assetName, string.Empty, onDone);
        }
        /// <summary>
        /// 非同步載入一個特定型別的 asset。
        /// </summary>
        /// <param name="category">Category.</param>
        /// <param name="assetName">Asset name.</param>
        /// <param name="variant">AssetBundle variant.</param>
        /// <param name="onDone">On done.</param>
        /// <typeparam name="T">The type derived from UnityEngine.Object.</typeparam>
        public void LoadAsync<T>(string category, string assetBundleName, string assetName, string variant, Action<T> onDone)
            where T : Object
        {
            string fullAssetBundleName;
            if (string.IsNullOrEmpty(assetBundleName))
                fullAssetBundleName = GetAssetBundleName(category, assetName, variant);
            else
                fullAssetBundleName = GetAssetBundleName(category, assetBundleName, variant);
            var assetLoadingOperation = assetManager.LoadAssetAsync(fullAssetBundleName, assetName, typeof(T));
            assetLoadingOperation.AssetLoaded +=
                asset => AssetManager_OnAssetLoaded(asset, category, fullAssetBundleName, assetName, variant, onDone);
        }
        
        void AssetManager_OnAssetLoaded<T>(
            Object asset,
            string category,
            string assetBundleName,
            string assetName,
            string variant,
            Action<T> onDone)
            where T : Object
        {
            var hasFallbackProvider = fallbackAssetProviders.Count > 0;
            var isTypeMatched = asset is T;
            var isNeedToLoadFromFallback = asset == null || !isTypeMatched;

            // 如果沒抓到，就從 fallback provider 開始找。
            if (hasFallbackProvider && isNeedToLoadFromFallback)
            {
                Observable
                    .FromMicroCoroutine(() => TryLoadFromFallbackProviders(
                        category,  assetName, variant, onDone))
                    .Subscribe();
            }
            else
            {
                OnAssetLoaded(asset as T, category, assetBundleName, assetName, onDone);
            }
        }

        static void OnAssetLoaded<T>(
            T asset,
            string category,
            string assetBundleName,
            string assetName,
            Action<T> onDone)
            where T : Object
        {
            if (asset == null)
            {
                logger.Error("Couldn't load {0} from {1} (category: {2}). Asset is null.",
                             assetName, assetBundleName, category);
            }

            onDone.Raise(asset);
        }

        IEnumerator TryLoadFromFallbackProviders<T>(
            string category,
            string assetName,
            string variant,
            Action<T> onDone)
            where T : Object
        {
            T fallbackAsset = null;
            var isResponded = false;
            var assetBundleName = string.Empty;

            // 一個一個試，試到有載到，或試到全部跑完。
            foreach (var fallbackAssetProvider in fallbackAssetProviders)
            {
                assetBundleName = fallbackAssetProvider.GetAssetBundleName(category, assetName, variant);
                fallbackAssetProvider.LoadAsync<T>(category, assetBundleName, assetName, variant, asset =>
                {
                    fallbackAsset = asset;
                    isResponded = true;
                });

                // 等待 fallback provider 回應。
                while (!isResponded)
                {
                    yield return null;
                }

                // 有載到就不用繼續嘗試下一個 fallback provider。
                if (fallbackAsset != null)
                {
                    break;
                }

                // 重置狀態，跑下一個 fallback provider。
                isResponded = false;
            }

            OnAssetLoaded(fallbackAsset, category, assetBundleName, assetName, onDone);
        }

        string GetAssetBundleName(string category, string assetName, string variant)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                logger.Error("You've try to get AssetBundle name from category \"{0}\". But asset name is empty.",
                    category);
                return null;
            }

            var assetBundleName = assetName.ToLower();
            if (!string.IsNullOrEmpty(variant))
            {
                assetBundleName = string.Concat(assetBundleName, ".", variant);
            }

            if (string.IsNullOrEmpty(category))
            {
                return assetBundleName;
            }

            AssetSettings.Entry entry;
            cachedSettings.TryGetValue(category, out entry);

            if (entry == null)
            {
                logger.Error("Category {0} not found.", category);
                return assetBundleName;
            }

            if (string.IsNullOrEmpty(entry.DirectoryName))
            {
                return assetBundleName;
            }

            // 斜線 '/' 在 cache & normalize 階段就完成了，不用再加
            // assetbundle name 只有小寫，避免操作上發生問題做全轉小寫
            return string.Concat(entry.DirectoryName, assetBundleName).ToLower();
        }

        public void UnloadAsset(Object Obj)
        {
            assetManager.UnloadAsset(Obj);
            Obj = null;
        }

        public void UnloadAssetBundle(string sAssetBundleName)
        {
            if (string.IsNullOrEmpty(sAssetBundleName))
                return;

            assetManager.UnloadAssetBundle(sAssetBundleName);
        }
    }
}

