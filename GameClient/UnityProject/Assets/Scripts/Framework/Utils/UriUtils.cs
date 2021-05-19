namespace Tizsoft.Utils
{
	public static class UriUtils
	{
	    /// <summary>
	    /// 回傳組合後的 URI，會自動處理斜線。
	    /// </summary>
	    /// <param name="host"></param>
	    /// <param name="path"></param>
	    /// <param name="query"></param>
	    /// <returns></returns>
	    public static string BuildUri(string host, string path, string query = null)
        {
            if (string.IsNullOrEmpty(host) && string.IsNullOrEmpty(path))
            {
                return "/";
            }

            if (string.IsNullOrEmpty(host))
            {
                return path.StartsWith("/") ? path : string.Concat("/", path);
            }

            if (string.IsNullOrEmpty(path))
            {
                return host.EndsWith("/") ? host : string.Concat(host, "/");
            }

            if (host.EndsWith("/") && path.StartsWith("/"))
            {
                host = host.Remove(host.Length - 2, 1);
            }
            else if (!host.EndsWith("/") && !path.StartsWith("/"))
            {
                path = string.Concat("/", path);
            }

            var uri = string.Concat(host, path);

            if (!string.IsNullOrEmpty(query))
            {
                uri = query.StartsWith("?")
                    ? string.Concat(uri, query)
                    : string.Concat(uri, "?", query);
            }

            return uri;
        }
    }
}
