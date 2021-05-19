#define Q9996 // UnloadAssetBundle Nick 20180611

using System;
using System.Collections.Generic;
using Tizsoft.Log;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tizsoft.AssetManagement
{
    public class FinishedAssetLoadingOperation : AssetLoadingOperation
    {
        static readonly Log.Logger logger = LogManager.Default.FindOrCreateLogger<FinishedAssetLoadingOperation>();

        public WeakReference MainAsset { get; set; }

        public List<WeakReference> SubAssets { get; set; }

        public FinishedAssetLoadingOperation()
        {
            SubAssets = new List<WeakReference>();
        }

        public bool IsAlive
        {
            get
            {
                if (MainAsset == null)
                {
                    return false;
                }

                if (!MainAsset.IsAlive || MainAsset.Target is GameObject || MainAsset.Target is Material)
                {
                    if (SubAssets == null || SubAssets.Count == 0)
                    {
#if Q9996
                        return false;
#else
                        return MainAsset.IsAlive;
#endif //Q9996
                    }

                    foreach (var sub in SubAssets)
                    {
                        if (sub.IsAlive)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                return MainAsset.IsAlive;
            }
        }

        public override event Action<Object> AssetLoaded
        {
            add
            {
                if (value != null)
                {
                    value(GetAsset());
                }
            }
            remove
            {
                // Do nothing
            }
        }

        public override Object GetAsset()
        {
            if (MainAsset.IsAlive)
            {
                if (MainAsset.Target == null)
                {
                    logger.Error("Unknown error occurred: Target reference has been released. assetBundleName={0}, assetName={1}", AssetBundleName, AssetName);
                }
                else
                {
                    return MainAsset.Target as Object;
                }
            }

            logger.Trace("Asset reference is dead. assetBundleName={0}, assetName={1}", AssetBundleName, AssetName);
            return base.GetAsset();
        }
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

// 此物件記錄的assetbundle都必須已完成下載
