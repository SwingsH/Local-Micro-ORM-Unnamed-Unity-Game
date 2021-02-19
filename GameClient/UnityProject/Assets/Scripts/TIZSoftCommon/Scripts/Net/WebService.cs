using System;

namespace TIZSoft.Net
{
    /// <summary>
    /// 表示一個 Web Service，提供基礎 HTTP 通訊實作。
    /// </summary>
    public class WebService
    {
        /// <summary>
        /// Gets the HTTP manager.
        /// </summary>
        /// <value>The http manager.</value>
        public HttpManager HttpManager { get; private set; }

        /// <summary>
        /// Gets the host manager.
        /// </summary>
        /// <value>The host manager.</value>
        public HostManager HostManager { get; private set; }

        public WebService(HttpManager httpManager, HostManager hostManager)
        {
            HttpManager = httpManager;
            HostManager = hostManager;
        }


        #region HEAD

        public HttpRequest Head(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Head(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Head(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Head, data, onResponded);
        }

        public HttpRequest Head(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Head, data, onResponded);
        }

        #endregion HEAD


        #region GET

        public HttpRequest Get(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Get(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Get(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Get, data, onResponded);
        }

        public HttpRequest Get(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Get, data, onResponded);
        }

        #endregion GET


        #region POST

        public HttpRequest Post(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Post(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Post(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Post, data, onResponded);
        }

        public HttpRequest Post(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Post, data, onResponded);
        }

        #endregion POST


        #region PUT

        public HttpRequest Put(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Put(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Put(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Put, data, onResponded);
        }

        public HttpRequest Put(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Put, data, onResponded);
        }

        #endregion PUT


        #region CREATE

        public HttpRequest Create(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Create(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Create(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Create, data, onResponded);
        }

        public HttpRequest Create(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Create, data, onResponded);
        }

        #endregion CREATE


        #region DELETE

        public HttpRequest Delete(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Delete(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Delete(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Delete, data, onResponded);
        }

        public HttpRequest Delete(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Delete, data, onResponded);
        }

        #endregion DELETE


        #region PATCH

        public HttpRequest Patch(string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Patch(HostManager.CurrentHostId, api, data, onResponded);
        }

        public HttpRequest Patch(string hostId, string api, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Patch, data, onResponded);
        }

        public HttpRequest Patch(string hostId, string api, string query, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Patch, data, onResponded);
        }

        #endregion PATCH



        public HttpRequest Call(string api, string httpMethod, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(HostManager.CurrentHostId, api, httpMethod, data, onResponded);
        }

        public HttpRequest Call(string api, string query, string httpMethod, byte[] data, Action<HttpRequest> onResponded)
        {
            return Call(HostManager.CurrentHostId, api, query, httpMethod, data, onResponded);
        }

        public HttpRequest Call(
            string hostId,
            string api,
            string query,
            string httpMethod,
            byte[] data,
            Action<HttpRequest> onResponded)
        {
            return Call(hostId, api, query, httpMethod, "application/octet-stream", data, onResponded);
        }

        public HttpRequest Call(
            string hostId,
            string api,
            string query,
            string httpMethod,
            string contentType,
            byte[] data,
            Action<HttpRequest> onResponded)
        {
            string host;
            if (!HostManager.TryFindHost(hostId, out host))
            {
                throw new ArgumentException(string.Format("Host ID {0} not found.", hostId));
            }

            var requestInfo = new HttpRequestInfo
            {
                Host = host,
                Path = api,
                Query = query,
                HttpMethod = httpMethod,
                ContentType = contentType,
                RequestData = data,
                OnResponded = onResponded
            };
            return HttpManager.Request(requestInfo);
        }
    }
}
