using System.Collections.Generic;
using TIZSoft.Versioning;
using UnityEditor;
using UnityEngine;

namespace TIZSoft.BuildSystem
{
    public class VersionEditorWindow : EditorWindow
    {
        SerializedProperty buildVersionProperty;
        SerializedObject serializedVersionSettings;
        VersionSettings versionSettings;

        void OnEnable()
        {
            LoadVersionSettings();
        }

        void OnDisable()
        {
            versionSettings = null;
        }

        void LoadVersionSettings()
        {
            if (versionSettings == null)
            {
                versionSettings = GetVersionSettings();
                serializedVersionSettings = new SerializedObject(versionSettings);
                buildVersionProperty = serializedVersionSettings.FindProperty("buildVersion");
            }
        }

        void OnGUI()
        {
            const string displayVersionDescription =
                @"顯示給終端使用者的版號，不是真正的建置版號。
等同於
Android 的 android:versionName，
iOS 的 CFBundleShortVersionString";
            EditorGUILayout.HelpBox(displayVersionDescription, MessageType.Info);
            PlayerSettings.bundleVersion = DrawVersion("Display Version", PlayerSettings.bundleVersion, true);

            EditorGUILayout.Separator();

            const string buildVersionDescription =
                @"真正的建置版號，內部開發人員用，作為唯一識別碼，不能與曾經上架過的建置版本號重複。
等同於
Android 的 android:versionCode
iOS 的 CFBundleVersion";
            EditorGUILayout.HelpBox(buildVersionDescription, MessageType.Info);
            PlayerSettings.iOS.buildNumber = DrawVersion("Build Version", PlayerSettings.iOS.buildNumber, false);
            PlayerSettings.Android.bundleVersionCode = VersionManager.ComputeVersionCode(PlayerSettings.iOS.buildNumber);

            // 將修改後的資訊寫到設定檔。
            LoadVersionSettings();
            if (serializedVersionSettings != null)
            {
                serializedVersionSettings.Update();
                buildVersionProperty.stringValue = PlayerSettings.iOS.buildNumber;
                serializedVersionSettings.ApplyModifiedProperties();
            }

            GUI.enabled = false;
            EditorGUILayout.IntField("Version Code", PlayerSettings.Android.bundleVersionCode);
            GUI.enabled = true;

            if (GUILayout.Button("Select Version Settings Asset", GUILayout.Width(200F)))
            {
                Selection.activeObject = versionSettings;
                EditorGUIUtility.PingObject(versionSettings);
            }
        }

        static VersionSettings GetVersionSettings()
        {
            var candidates = AssetDatabase.FindAssets("t:VersionSettings");
            if (candidates.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(candidates[0]);
                return AssetDatabase.LoadAssetAtPath<VersionSettings>(assetPath);
            }

            var versionSettings = CreateInstance<VersionSettings>();
            AssetDatabase.CreateAsset(versionSettings, "Assets/Resources/version_settings.asset");
            AssetDatabase.SaveAssets();
            return versionSettings;
        }

        static string DrawVersion(string labelName, string version, bool isIdentifierAllowed)
        {
            var components = VersionManager.GetVersionComponents(version);
            string newVersion;

            EditorGUILayout.LabelField(labelName);
            EditorGUILayout.BeginVertical();
            {
                DrawVersionComponent("Major", components, VersionManager.MajorIndex, VersionComponentType.Integer);
                DrawVersionComponent("Minor", components, VersionManager.MinorIndex, VersionComponentType.Integer);
                DrawVersionComponent("Patch", components, VersionManager.PatchIndex, VersionComponentType.Integer);
                if (isIdentifierAllowed)
                {
                    DrawVersionComponent("Identifier", components, VersionManager.IdentifierIndex, VersionComponentType.String);
                }

                newVersion = VersionManager.ComposeVersionString(components);

                var prevGuiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.TextField("Version", newVersion);
                GUI.enabled = prevGuiEnabled;
            }
            EditorGUILayout.EndVertical();

            return newVersion;
        }

        static void DrawVersionComponent(
            string labelName, IList<string> components, int index, VersionComponentType componentType)
        {
            if (components.Count <= index)
            {
                return;
            }

            var options = GUILayout.Width(240F);
            if (componentType == VersionComponentType.Integer)
            {
                int number;
                int.TryParse(components[index], out number);
                number = EditorGUILayout.IntField(labelName, number, options);
                number = Mathf.Clamp(number, 0, int.MaxValue);
                components[index] = number.ToString();
            }
            else if (componentType == VersionComponentType.String)
            {
                components[index] = EditorGUILayout.TextField(labelName, components[index], options);
            }
        }

        enum VersionComponentType
        {
            Integer,
            String
        }
    }
}