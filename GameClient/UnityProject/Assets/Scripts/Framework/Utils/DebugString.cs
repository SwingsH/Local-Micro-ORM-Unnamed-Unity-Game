
using System.Collections.Generic;

namespace TIZSoft.Utils
{
	public static class DebugString
	{
        public static string Get(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            string result = string.Empty;
            foreach (var kValue in keyValues)
            {
                result += string.Format("[{0}:{1}]", kValue.Key, kValue.Value);
            }
            return result;
        }

        public static string Get(this Dictionary<string, string> keyValues)
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