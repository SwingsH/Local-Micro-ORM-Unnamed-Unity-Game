using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace TIZSoft.UnityHTTP.Client
{
    /// <summary>
    /// URL 查詢字串（或參數字串）建構工具類別。
    /// </summary>
    public class QueryStringBuilder
    {
        readonly List<KeyValuePair<string, object>> args = new List<KeyValuePair<string, object>>();

        public QueryStringBuilder(params KeyValuePair<string, object>[] args)
        {
            Append(args);
        }

        public QueryStringBuilder(IEnumerable<KeyValuePair<string, object>> args)
        {
            Append(args);
        }

        public QueryStringBuilder Append(string key, object value)
        {
            args.Add(new KeyValuePair<string, object>(key, value));
            return this;
        }

        public QueryStringBuilder Append(KeyValuePair<string, object> keyValuePair)
        {
            args.Add(keyValuePair);
            return this;
        }

        public QueryStringBuilder Append(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            if (keyValuePairs == null)
            {
                return this;
            }

            args.AddRange(keyValuePairs);
            return this;
        }

        public string Build(bool escapeUrl = true)
        {
            var builder = new StringBuilder();

            if (args.Count >= 1)
            {
                var kvp = args[0];
                builder.AppendFormat("{0}={1}", kvp.Key, kvp.Value);
            }
            for (var i = 1; i < args.Count; ++i)
            {
                var kvp = args[i];
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    continue;
                }

                var value = kvp.Value != null ? kvp.Value.ToString() : string.Empty;
#if UNITY_2020_2_OR_NEWER
                builder.AppendFormat("&{0}={1}", kvp.Key, UnityWebRequest.EscapeURL(value));
#else
                builder.AppendFormat("&{0}={1}", kvp.Key, WWW.EscapeURL(value));
#endif
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
