using System;
using UnityEngine.Networking;

namespace Tizsoft.Net
{
    /// <summary>
    /// 表示一個 Web Service，提供基礎 HTTP 通訊實作。
    /// </summary>
    public class ClientWebServices
    {
        /// <summary>
        /// Gets the HTTP manager.
        /// </summary>
        /// <value>The http manager.</value>
        public ClientHttpManager HttpManager { get; private set; }

        /// <summary>
        /// Gets the host manager.
        /// </summary>
        /// <value>The host manager.</value>
        public ClientHostManager HostManager { get; private set; }

        public ClientWebServices(ClientHttpManager httpManager, ClientHostManager hostManager)
        {
            HttpManager = httpManager;
            HostManager = hostManager;
        }


        #region HEAD

        public ClientHttpRequest Head(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Head(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Head(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Head, data, onResponded);
        }

        public ClientHttpRequest Head(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Head, data, onResponded);
        }

        #endregion HEAD


        #region GET

        public ClientHttpRequest Get(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Get(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Get(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Get, data, onResponded);
        }

        public ClientHttpRequest Get(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Get, data, onResponded);
        }

        #endregion GET


        #region POST

        public ClientHttpRequest Post(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Post(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Post(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Post, data, onResponded);
        }

        public ClientHttpRequest Post(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Post, data, onResponded);
        }

        #endregion POST


        #region PUT

        public ClientHttpRequest Put(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Put(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Put(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Put, data, onResponded);
        }

        public ClientHttpRequest Put(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Put, data, onResponded);
        }

        #endregion PUT


        #region CREATE

        public ClientHttpRequest Create(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Create(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Create(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Create, data, onResponded);
        }

        public ClientHttpRequest Create(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Create, data, onResponded);
        }

        #endregion CREATE


        #region DELETE

        public ClientHttpRequest Delete(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Delete(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Delete(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Delete, data, onResponded);
        }

        public ClientHttpRequest Delete(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Delete, data, onResponded);
        }

        #endregion DELETE


        #region PATCH

        public ClientHttpRequest Patch(string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Patch(HostManager.CurrentHostId, api, data, onResponded);
        }

        public ClientHttpRequest Patch(string hostId, string api, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, HttpMethod.Patch, data, onResponded);
        }

        public ClientHttpRequest Patch(string hostId, string api, string query, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, HttpMethod.Patch, data, onResponded);
        }

        #endregion PATCH



        public ClientHttpRequest Call(string api, string httpMethod, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(HostManager.CurrentHostId, api, httpMethod, data, onResponded);
        }

        public ClientHttpRequest Call(string api, string query, string httpMethod, byte[] data, Action<ClientHttpRequest> onResponded)
        {
            return Call(HostManager.CurrentHostId, api, query, httpMethod, data, onResponded);
        }

        public ClientHttpRequest Call(
            string hostId,
            string api,
            string query,
            string httpMethod,
            byte[] data,
            Action<ClientHttpRequest> onResponded)
        {
            return Call(hostId, api, query, httpMethod, "application/octet-stream", data, onResponded);
        }

        public ClientHttpRequest Call(
            string hostId,
            string api,
            string query,
            string httpMethod,
            string contentType,
            byte[] data,
            Action<ClientHttpRequest> onResponded)
        {
            string host;
            if (!HostManager.TryFindHost(hostId, out host))
            {
                throw new ArgumentException(string.Format("Host ID {0} not found.", hostId));
            }

            var requestInfo = new ClientHttpRequestInfo
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
public static class HttpMethod
{
    public const string Get = UnityWebRequest.kHttpVerbGET;
    public const string Put = UnityWebRequest.kHttpVerbPUT;
    public const string Head = UnityWebRequest.kHttpVerbHEAD;
    public const string Post = UnityWebRequest.kHttpVerbPOST;
    public const string Create = UnityWebRequest.kHttpVerbCREATE;
    public const string Delete = UnityWebRequest.kHttpVerbDELETE;
    public const string Patch = "PATCH";
}