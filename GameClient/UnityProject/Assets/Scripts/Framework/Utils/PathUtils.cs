using System.IO;

namespace TIZSoft.Utils
{
    /// <summary>
    /// 路徑工具方法。
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// 仿 .NET 4.0 以上的 Path API，將一個字串陣列合併為單一路徑。
        /// </summary>
        /// <param name="paths">路徑中各部分的陣列。</param>
        /// <returns>合併的路徑。</returns>
        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0)
            {
                return string.Empty;
            }

            var combinedPath = paths[0];
            for (var i = 1; i < paths.Length; ++i)
            {
                combinedPath = Path.Combine(combinedPath, paths[i]);
            }
            return combinedPath;
        }
    }
}