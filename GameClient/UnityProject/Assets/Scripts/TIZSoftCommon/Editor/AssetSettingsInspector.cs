/*
using Rotorz.ReorderableList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TIZSoft.Extensions;
using UnityEditor;
using UnityEngine;

namespace TIZSoft
{
    /// <summary>
    /// Asset 設定編輯器，包含套用 AssetBundle Names。
    /// </summary>
    [CustomEditor(typeof(AssetSettings), true)]
	public class AssetSettingsInspector : Editor
	{
		SerializedProperty entries;

		[MenuItem("Tools/Open Asset Settings")]
		static void OpenAssetSettings()
		{
			var candidates = AssetDatabase.FindAssets("t:AssetSettings");
			if (candidates.Length > 0)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(candidates[0]);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<AssetSettings>(assetPath);
            }
		}

        void OnEnable()
        {
	        entries = serializedObject.FindProperty("entries");
        }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as AssetSettings;
			
            if (GUILayout.Button("Apply AssetBundle Names", GUILayout.Height(40)))
            {
                ApplyAssetBundleNames(t, -1);
            }
			
			// 繪製 Entries
            ReorderableListGUI.Title(entries.GetGuiContent());
            const int fieldCount = 4;
            var index = 0;
            ReorderableListGUI.ListField(new GenericListAdaptor<AssetSettings.Entry>(t.Entries, (position, item) =>
            {
                var height = position.height/fieldCount;
				
                position.height = height;
                item.Category = EditorGUI.TextField(position, "Category", item.Category ?? string.Empty);
                position.y += position.height;

                position.height = height;
                item.DirectoryName = EditorGUI.TextField(position, "Directory Name", item.DirectoryName ?? string.Empty);
                position.y += position.height;

                position.height = height;
                item.AssetBundleNameFormat = EditorGUI.TextField(position, "Bundle Name Format", item.AssetBundleNameFormat ?? string.Empty);
                position.y += position.height;

                const int buttonWidth = 180;
	            position.x += position.width - buttonWidth;
                position.width = buttonWidth;
                position.height = height;
	            if (GUI.Button(position, "Apply AssetBundle Names"))
	            {
		            ApplyAssetBundleNames(t, index);
	            }
                position.y += position.height;

                ++index;
                return item;
            }, ReorderableListGUI.DefaultItemHeight*fieldCount));

			EditorUtility.SetDirty(t);

            serializedObject.ApplyModifiedProperties();
		}

        // index -1 代表要 Apply All
        static void ApplyAssetBundleNames(AssetSettings settings, int index)
		{
            var processedEntries = new Dictionary<AssetSettings.Entry, string[]>();
			for (var i = 0; i < settings.Entries.Count; i++)
			{
				if (i != -1 && i != index)
				{
					continue;
				}

				var entry = settings.Entries[i];
				var dirName = Path.Combine("Assets", entry.DirectoryName);

				// 允許一包 asset bundle 多個 assets
				var paths = Directory.GetFiles(dirName, "*", SearchOption.TopDirectoryOnly)
					.Concat(Directory.GetDirectories(dirName, "*", SearchOption.TopDirectoryOnly))
					.Where(path => !path.EndsWith(".meta"))
					.Select(path => path.Replace('\\', '/'));
                processedEntries.Add(entry, paths.ToArray());
			}

			var count = 0;
			var totalCount = processedEntries.Sum(kvp => kvp.Value.Length);
			if (totalCount == 0)
			{
				return;
			}

			const string progressBarTitle = "Set AssetBundle Names...";
            foreach (var kvp in processedEntries)
			{
				var entry = kvp.Key;
				var assetPaths = kvp.Value;

				foreach (var assetPath in assetPaths)
				{
                    if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, assetPath, (float)count++/totalCount))
					{
						break;
					}

                    var assetImporter = AssetImporter.GetAtPath(assetPath);
					var lowerAssetPath = assetPath.ToLowerInvariant();
					var dotIndex = lowerAssetPath.LastIndexOf(".", StringComparison.InvariantCulture);
					if (dotIndex >= 0)
					{
                        lowerAssetPath = lowerAssetPath.Substring(0, dotIndex);
					}
                    lowerAssetPath = lowerAssetPath.Remove(0, "assets/".Length);

                    var assetBundleName = string.IsNullOrEmpty(entry.AssetBundleNameFormat)
                                              ? "${Path}"
                                              : entry.AssetBundleNameFormat;
                    // Replace symbols
                    assetBundleName = assetBundleName.Replace("${Path}", lowerAssetPath);
					assetBundleName = assetBundleName.Replace("${Category}", entry.Category);
                    assetBundleName = assetBundleName.Replace("${Name}", Path.GetFileNameWithoutExtension(assetPath));

                    if (assetImporter.assetBundleName != assetBundleName)
					{
						assetImporter.SetAssetBundleNameAndVariant(assetBundleName, string.Empty);
					}

                    if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, assetPath, (float)count/totalCount))
					{
						break;
					}
				}
			}
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }
	}
}
*/