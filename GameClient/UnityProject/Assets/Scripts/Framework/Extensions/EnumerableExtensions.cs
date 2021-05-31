using System.Collections.Generic;

namespace TIZSoft.Extensions
{
    public static class EnumerableExtensions
    {
        public static string DebugString(this IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            string result = string.Empty;
            foreach (var kValue in keyValues)
            {
                result += string.Format("[{0}:{1}]", kValue.Key, kValue.Value);
            }
            return result;
        }

        public static string DebugString(this Dictionary<string, string> keyValues)
        {
            string result = string.Empty;
            foreach (var kValue in keyValues)
            {
                result += string.Format("[{0}:{1}]", kValue.Key, kValue.Value);
            }
            return result;
        }
    }
}
