using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
#endif

namespace Tizsoft
{
    /// <summary>
    /// AssetBundle 設定。
    /// </summary>
	[CreateAssetMenu(fileName = "asset_settings", menuName = "Asset Settings", order = 1)]
    public class AssetSettings : ScriptableObject
	{
        [Serializable]
        public class Entry
        {
            [SerializeField]
            [Tooltip("這類遊戲 asset 的分類名稱。e.g. Background")]
            string category;

            [SerializeField]
            [Tooltip("AssetBundle 所在的目錄名稱。e.g. cg/01_bg_talk/prefab/")]
            string directoryName;

            [SerializeField]
            [Tooltip("AssetBundle Name 的命名規則。不指定的話預設為 path 不含副檔名。\n" +
                     "e.g. cg/01_bg_talk/prefab/bg_talk_000\n" +
                     "有下列符號可使用：\n" +
                     "${Path} 代表全小寫的 asset path，不含 assets/ 及副檔名。" +
                     "${Category} 代表此 entry 的 Category。\n" +
                     "${Name} 代表此 entry 下抓到的檔名或目錄名，不含副檔名。\n" +
                     "e.g. cg/01_bg_talk/${Name}")]
            string assetBundleNameFormat;

            [SerializeField]
            [Tooltip("變更檔名 ${Name} 的規則表示式\n" +
                     "AssetBundleNameFormat 不為空值才有效\n" +
                     "固定變更為 Group 1 的值" )]
            string fileNameNormalizeRegex;

            /// <summary>
            /// 這類遊戲 asset 的分類名稱。e.g. Background
            /// </summary>
            public string Category
            {
                get { return category; }
                set { category = value; }
            }

            /// <summary>
            /// AssetBundle 所在的目錄名稱。e.g. cg/01_bg_talk/prefab/
            /// </summary>
            public string DirectoryName
            {
                get { return directoryName; }
                set { directoryName = value.ToLower(); }
            }
            
            /// <summary>
            /// AssetBundle Name 的命名規則。不指定的話預設為 path 不含副檔名。<br />
            /// e.g. cg/01_bg_talk/prefab/bg_talk_000<br />
            /// 有下列符號可使用：<br />
            /// ${Path} 代表全小寫的 asset path，不含 assets/ 及副檔名。<br />
            /// ${Category} 代表此 entry 的 Category。<br />
            /// ${Name} 代表此 entry 下抓到的檔名或目錄名，不含副檔名。
            /// </summary>
            public string AssetBundleNameFormat
            {
                get { return assetBundleNameFormat; }
                set { assetBundleNameFormat = value; }
            }

            /// <summary>
            /// 變更檔名 ${Name} 的規則表示式
            /// AssetBundleNameFormat 不為空值才有效
            /// 固定變更為 Group 1 的值
            /// </summary>
            public string FileNameNormalizeRegex
            {
                get { return fileNameNormalizeRegex; }
                set { fileNameNormalizeRegex = value.ToLower(); }
            }

#if UNITY_EDITOR
            public string GetAssetBundleName(string assetName)
            {
                var dirName = Path.Combine("Assets", DirectoryName);

                // 允許一包 asset bundle 多個 assets
                var entry = this;
                var assetPaths = Directory.GetFiles(dirName, "*", SearchOption.TopDirectoryOnly)
                    .Concat(Directory.GetDirectories(dirName, "*", SearchOption.TopDirectoryOnly))
                    .Where(path => !path.EndsWith(".meta"))
                    .Select(path => path.Replace('\\', '/'));
                foreach (var assetPath in assetPaths)
                {
                    var lowerAssetPath = assetPath.ToLowerInvariant();
                    if (Path.GetFileNameWithoutExtension(assetPath) != assetName)
                    {
                        continue;
                    }

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
                    var fileName = Path.GetFileNameWithoutExtension(assetPath);
                    if(!string.IsNullOrEmpty(assetBundleName)
                        && !string.IsNullOrEmpty(fileNameNormalizeRegex))
                    {
                        var nameRegex = new System.Text.RegularExpressions.Regex(fileNameNormalizeRegex);
                        if(nameRegex.IsMatch(fileName))
                        {
                            var match = nameRegex.Match(fileName);
                            if (match.Groups.Count > 1)
                                fileName = match.Groups[1].ToString();
                        }
                    }
                    assetBundleName = assetBundleName.Replace("${Name}", fileName);
                    return assetBundleName;
                }
                return string.Empty;
            }
#endif
        }
        
        [SerializeField]
        List<Entry> entries = new List<Entry>();

	    public IList<Entry> Entries
	    {
	        get { return entries; }
	    }
    }
}
