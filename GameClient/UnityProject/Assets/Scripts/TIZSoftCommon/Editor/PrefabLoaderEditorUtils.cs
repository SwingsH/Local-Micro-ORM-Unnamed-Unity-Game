using System;
using TIZSoft.Extensions;
using UnityEditor;
using UnityEngine;

namespace TIZSoft
{
    public static class PrefabLoaderEditorUtils
    {
        public static void SaveNestedPrefab(PrefabLoader prefabLoader)
        {
            if (prefabLoader == null)
            {
                return;
            }

            // 根據 GameObject 的 prefab 種類來決定要如何處理
            var prefabType = PrefabUtility.GetPrefabType(prefabLoader.gameObject);
            switch (prefabType)
            {
                // 還不是 prefab 或 prefab 已經丟失
                case PrefabType.None:
                case PrefabType.MissingPrefabInstance:
                    {
                        // 將還不是 prefab 的物件存到指定的路徑
                        var pathToSave = EditorUtility.SaveFilePanelInProject("Save Prefab",
                                                                              prefabLoader.name,
                                                                              "prefab",
                                                                              "Save Prefab To");
                        if (!string.IsNullOrEmpty(pathToSave))
                        {
                            // 先解除所有子物件
                            var children = prefabLoader.transform.GetChildren(TransformSearchOption.TopOnly);
                            foreach (var child in children)
                            {
                                child.SetParent(null, false);
                            }

                            // 存
                            PrefabUtility.CreatePrefab(pathToSave,
                                                       prefabLoader.gameObject,
                                                       ReplacePrefabOptions.ConnectToPrefab);

                            // 把子物件加回去
                            foreach (Transform child in children)
                            {
                                child.SetParent(prefabLoader.transform, false);
                            }
                        }
                    }
                    break;
                // 是在 Assets/ 內，不用進行任何調整
                case PrefabType.Prefab:
                case PrefabType.ModelPrefab:
                    {
                    }
                    break;
                // 是 prefab instance，將場景上修改後的 prefab instance 取代原本的 prefab
                case PrefabType.PrefabInstance:
                case PrefabType.DisconnectedPrefabInstance:
                case PrefabType.ModelPrefabInstance:
                case PrefabType.DisconnectedModelPrefabInstance:
                    {
                        // 先解除所有子物件
                        var children = prefabLoader.transform.GetChildren(TransformSearchOption.TopOnly);
                        foreach (var child in children)
                        {
                            child.SetParent(null, false);
                        }

                        // 存
                        var originalPrefab = PrefabUtility.GetPrefabParent(prefabLoader.gameObject);
                        PrefabUtility.ReplacePrefab(prefabLoader.gameObject,
                                                    originalPrefab,
                                                    ReplacePrefabOptions.ConnectToPrefab);

                        // 把子物件加回去
                        foreach (Transform child in children)
                        {
                            child.SetParent(prefabLoader.transform, false);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("prefabLoader", string.Format("Undefined value: {0}", prefabType));
            }
        }
    }
}
