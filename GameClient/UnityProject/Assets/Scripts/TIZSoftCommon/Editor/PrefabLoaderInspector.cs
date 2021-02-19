using System;
using System.Linq;
using TIZSoft.Extensions;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace TIZSoft
{
    // TODO 需要在 Hierarchy 上做一些更方便操作的視窗工具
    // TODO 可能要把一些常用的 editor 方法、類別或實作抽出來作為共用程式庫。
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PrefabLoader))]
    public class PrefabLoaderInspector : Editor
    {
        static class PropertyNames
        {
            public const string Timing = "timing";
            public const string SourceType = "sourceType";
            public const string WorldPositionStays = "worldPositionStays";
            public const string Prefab = "prefab";
            public const string AssetProviderName = "assetProviderName";
            public const string Category = "category";
            public const string AssetName = "assetName";
            public const string IntValue = "intValue";
            public const string OnLoadCompleted = "onLoadCompleted";
        }
        
        static readonly GUIContent LoadButtonContent = new GUIContent("Load", "將 prefab 載入至場景上。");
        static readonly GUIContent UnloadButtonContent = new GUIContent("Unload", "卸載所有子物件，包含已載入的 prefab instance。");
        
        SerializedProperty prefabLoadingTimingProperty;
        SerializedProperty sourceTypeProperty;
        SerializedProperty worldPositionStaysProperty;
        SerializedProperty prefabProperty;
        SerializedProperty assetProviderNameProperty;
        SerializedProperty categoryProperty;
        SerializedProperty assetNameProperty;
        SerializedProperty intValueProperty;
        SerializedProperty onLoadCompletedProperty;
        
        void OnEnable()
        {
            prefabLoadingTimingProperty = serializedObject.FindProperty(PropertyNames.Timing);
            sourceTypeProperty = serializedObject.FindProperty(PropertyNames.SourceType);
            worldPositionStaysProperty = serializedObject.FindProperty(PropertyNames.WorldPositionStays);
            prefabProperty = serializedObject.FindProperty(PropertyNames.Prefab);
            assetProviderNameProperty = serializedObject.FindProperty(PropertyNames.AssetProviderName);
            categoryProperty = serializedObject.FindProperty(PropertyNames.Category);
            assetNameProperty = serializedObject.FindProperty(PropertyNames.AssetName);
            intValueProperty = serializedObject.FindProperty(PropertyNames.IntValue);
            onLoadCompletedProperty = serializedObject.FindProperty(PropertyNames.OnLoadCompleted);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var prefabLoaders = serializedObject.targetObjects.Cast<PrefabLoader>();

            GUILayout.BeginHorizontal();
            DrawButton(LoadButtonContent, Color.green, EditorStyles.miniButtonLeft, () =>
            {
                foreach (var prefabLoader in prefabLoaders)
                {
                    prefabLoader.LoadAsync().Subscribe();
                }
            });
            DrawButton(UnloadButtonContent, Color.red, EditorStyles.miniButtonRight, () =>
            {
                foreach (var prefabLoader in prefabLoaders)
                {
                    prefabLoader.Unload();
                }
            });
            GUILayout.EndHorizontal();
            
            // loadingTiming
            // 疑似是 Unity editor bug，Enum 得修改後馬上儲存，否則修改執會無效
            EditorGUILayout.PropertyField(prefabLoadingTimingProperty);
            prefabLoadingTimingProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorGUILayout.PropertyField(sourceTypeProperty);
            sourceTypeProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorGUILayout.PropertyField(worldPositionStaysProperty);

            if (sourceTypeProperty.enumValueIndex == (int)PrefabLoader.PrefabSourceType.FromPrefab)
            {
                EditorGUILayout.PropertyField(prefabProperty);
                assetProviderNameProperty.stringValue = string.Empty;
                categoryProperty.stringValue = string.Empty;
                assetNameProperty.stringValue = string.Empty;
            }
            else if (sourceTypeProperty.enumValueIndex == (int)PrefabLoader.PrefabSourceType.FromAssetBundle)
            {
                prefabProperty.objectReferenceValue = null;
                EditorGUILayout.PropertyField(assetProviderNameProperty);
                EditorGUILayout.PropertyField(categoryProperty);
                EditorGUILayout.PropertyField(assetNameProperty);
            }
            
            EditorGUILayout.PropertyField(intValueProperty);

            EditorGUILayout.Separator();

            // onLoadCompleted
            EditorGUILayout.PropertyField(onLoadCompletedProperty);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawButton(GUIContent content, Color color, GUIStyle style, Action onClick, params GUILayoutOption[] layoutOptions)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(content, style ?? GUIStyle.none, layoutOptions))
            {
                onClick.Raise();
            }
            GUI.backgroundColor = prevColor;
        }

        void DrawButton(Rect rect, string text, Color color, GUIStyle style, Action onClick)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUI.Button(rect, text, style ?? GUIStyle.none))
            {
                onClick.Raise();
            }
            GUI.backgroundColor = prevColor;
        }

        void DrawFieldWithReset(Rect rect,
                                SerializedProperty serializedProperty,
                                Func<SerializedProperty, bool> resetableTester,
                                Action<SerializedProperty> onClick)
        {
            const float buttonWidth = 40F;
            const float horizontalPadding = 2F;

            // Field
            var propRect = new Rect(rect.x, rect.y, rect.width - buttonWidth, rect.height);
            EditorGUI.PropertyField(propRect, serializedProperty);

            // Button
            var isResetable = resetableTester.Raise(serializedProperty);
            var color = isResetable ? Color.green : Color.white;
            GUI.enabled = isResetable;
            var buttonRect = new Rect(rect.x + rect.width - buttonWidth + horizontalPadding, rect.y, buttonWidth, rect.height);
            DrawButton(buttonRect, "Reset", color, GUI.skin.button, () => onClick.Raise(serializedProperty));
            GUI.enabled = true;
        }

        static string CreateUndoName(string actionName)
        {
            return typeof(PrefabLoader).Name + " " + actionName;
        }
    }
}
