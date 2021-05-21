using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using Tizsoft.Net;
using Tizsoft.Utils;

namespace Tizsoft.Net
{
    public class GameServices
    {
        readonly Dictionary<ServerType, string> serverIds = new Dictionary<ServerType, string>();

        /// <summary>
        /// 取得 API 前綴。e.g. "/account/create"，account 即為前綴。
        /// </summary>
        protected string Prefix { get; private set; }

        protected ClientWebServices WebService { get; private set; }

        protected ClientHTTPNetwork Network { get; set; }

        public GameServices(ClientWebServices webService, string prefix, ClientHTTPNetwork network)
        {
            WebService = webService;
            Prefix = prefix;
            Network = network;

            Network.ServerAdded.Subscribe(e =>
            {
                var serverId = CreateServerId(e.GroupName, e.ServerType, e.ServerIndex);
                serverIds[e.ServerType] = serverId;
                if (!WebService.HostManager.ContainsHostId(serverId))
                {
                    WebService.HostManager.AddHost(serverId, e.ServerUrl);
                }
            });
            Network.ServerChanged.Subscribe(e =>
            {
                var serverId = CreateServerId(e.GroupName, e.ServerType, e.ServerIndex);
                serverIds[e.ServerType] = serverId;
                WebService.HostManager.SetHost(serverId, e.ServerUrl);
            });
        }

        string FindServerId(ServerType serverType)
        {
            string serverId;
            serverIds.TryGetValue(serverType, out serverId);
            return serverId;
        }

        static string CreateServerId(string groupName, ServerType serverType, int index)
        {
            return string.Concat(groupName, serverType.ToString().ToLowerInvariant(), index.ToString());
        }

        string GetFullApiName(string api)
        {
            var fullApiName = "/";

            if (!string.IsNullOrEmpty(Prefix))
            {
                fullApiName += Prefix;
            }

            if (!string.IsNullOrEmpty(api))
            {
                if (!fullApiName.EndsWith("/"))
                {
                    fullApiName += "/";
                }

                fullApiName += api;
            }

            return fullApiName;
        }

        protected void Get(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Get, data, onResponse);
        }

        protected void Get(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Get, data, onResponse);
        }

        protected void Put(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Put, data, onResponse);
        }

        protected void Put(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Put, data, onResponse);
        }

        protected void Head(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Head, data, onResponse);
        }

        protected void Head(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Head, data, onResponse);
        }

        protected void Post(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Post, data, onResponse);
        }

        protected void Post(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Post, data, onResponse);
        }

        protected void Create(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Create, data, onResponse);
        }

        protected void Create(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Create, data, onResponse);
        }

        protected void Delete(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Delete, data, onResponse);
        }

        protected void Delete(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Delete, data, onResponse);
        }

        protected void Patch(
            ServerType serverType,
            string api,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, HttpMethod.Patch, data, onResponse);
        }

        protected void Patch(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, query, HttpMethod.Patch, data, onResponse);
        }

        protected void Call(
            ServerType serverType,
            string api,
            string httpMethod,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            Call(serverType, api, null, httpMethod, data, onResponse);
        }

        protected void Call(
            ServerType serverType,
            string api,
            QueryStringBuilder query,
            string httpMethod,
            object data,
            Action<ClientHttpRequest> onResponse)
        {
            var fullApi = GetFullApiName(api);
            try
            {
                var serverId = FindServerId(serverType);
                var json = data != null ? JObject.FromObject(data).ToString() : string.Empty;
                var bytes = Encoding.UTF8.GetBytes(json);

                //logger.Debug("HTTP {0} {1}", httpMethod.ToUpper(),UriUtils.BuildUri(WebService.HostManager.FindHost(serverId), fullApi));
                WebService.Call(
                    serverId,
                    fullApi,
                    query != null ? query.Build() : null,
                    httpMethod,
                    "application/json; charset=UTF-8",
                    bytes,
                    onResponse.Invoke);
            }
            catch (Exception e)
            {
                //logger.Error(e, "API={0}, HTTPMethod={1}", fullApi, httpMethod);
                throw;
            }
        }
    }
}

