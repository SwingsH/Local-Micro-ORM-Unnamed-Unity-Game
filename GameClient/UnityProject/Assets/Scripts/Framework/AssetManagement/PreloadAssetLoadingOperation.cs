using System;
using System.Collections;
using System.Collections.Generic;

namespace TIZSoft.AssetManagement
{
    public class PreloadAssetLoadingOperation : IEnumerator
    {
        event Action assetLoaded;

        public event Action AssetLoaded
        {
            add
            {
                if (assetLoadingOperations.Count == 0)
                {
                    if (value != null)
                    {
                        value();
                    }
                }
                else
                {
                    assetLoaded += value;
                }
            }
            remove
            {
                assetLoaded -= value;
            }
        }

        readonly List<AssetLoadingOperation> assetLoadingOperations;

        public object Current
        {
            get
            {
                return null;
            }
        }

        public PreloadAssetLoadingOperation(List<AssetLoadingOperation> assetLoadingOperations)
        {
            this.assetLoadingOperations = assetLoadingOperations;
        }

        public void OnAssetLoaded()
        {
            if (assetLoaded != null)
            {
                assetLoaded();
                assetLoaded = null;
            }
        }

        public bool IsDone
        {
            get
            {
                foreach (var loading in assetLoadingOperations)
                {
                    if (!loading.IsDone)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
            // Do nothing.
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
