using UnityEngine;

namespace TIZSoft.Utils
{
    /// <summary>
    /// Unity Editor 路徑工具。
    /// </summary>
    public static class UnityEditorPathUtils
    {
        /// <summary>
        /// 將完整路徑轉成 Unity project 的 asset path，供 <see cref="UnityEditor.AssetDatabase"/> 使用。
        /// e.g. "C:\Users\User\My Project\Assets\Prefabs\Ball.prefab" ->
        ///      "Assets\Prefabs\Ball.prefab"
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetAssetPath(string fullPath)
        {
            var startIndex = Application.dataPath.Length - "Assets".Length;
            return fullPath.Substring(startIndex, fullPath.Length - startIndex);
        }
    }
}
