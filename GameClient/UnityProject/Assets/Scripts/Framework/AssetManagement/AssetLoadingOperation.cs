using System;
using System.Collections;
using AssetBundles;
using Tizsoft.Log;
using UnityEngine;
using Logger = Tizsoft.Log.Logger;
using Object = UnityEngine.Object;

namespace Tizsoft.AssetManagement
{
    public class AssetLoadingOperation : IEnumerator
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<AssetLoadingOperation>();

        // Another coroutine is already waiting for this coroutine!
        // Currently only one coroutine can wait for another coroutine!
        // 這種方式會出現上面那種錯誤

        //MonoBehaviour monoinstance;

        //Coroutine coroutin;
        //public Coroutine Coroutine
        //{
        //    get { return this; }
        //}

        event Action<Object> assetLoaded;

        public AssetBundleLoadAssetOperation assetBundleLoadOperation;

        public string AssetName { get; set; }

        /// <summary>
        /// full related path
        /// </summary>
        public string AssetBundleName { get; set; }

        public Type AssetType { get; set; }

        public object Current
        {
            get
            {
                return null;
            }
        }

        public virtual event Action<Object> AssetLoaded
        {
            add
            {
                assetLoaded += value;
            }
            remove
            {
                assetLoaded -= value;
            }
        }

        public virtual bool IsDone
        {
            get
            {
                if (assetBundleLoadOperation == null)
                {
                    return false;
                }
                return assetBundleLoadOperation.IsDone();
            }
        }

        public void OnAssetLoaded()
        {
            if (assetLoaded != null)
            {
                assetLoaded(GetAsset());
            }
            assetLoaded = null;
        }

        public virtual Object GetAsset()
        {
            if (assetBundleLoadOperation != null)
            {
                var asset = assetBundleLoadOperation.GetAsset<Object>();
                if (asset == null)
                {
                    logger.Error("Asset is null. assetBundleName={0}, assetName={1}", AssetBundleName, AssetName);
                }

                return asset;
            }

            logger.Error("Asset bundle is null. assetBundleName={0}, assetName={1}", AssetBundleName, AssetName);
            return null;
        }

        public bool MoveNext()
        {
            // 等待排程下載，避免同時下載過多檔案時可能會發生問題
            if (assetBundleLoadOperation == null)
            {
                return true;
            }

            // 等待下載
            return !assetBundleLoadOperation.IsDone();
        }

        public void Reset()
        {
        }
    }
}

// 只支援單一物件assetbundle，多物件讀取釋放較複雜暫不處理
