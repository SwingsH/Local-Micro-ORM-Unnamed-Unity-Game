
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using TIZSoft.Extensions;

namespace TIZSoft.UnityHTTP.Client
{
    public class HttpEventArgs
    {
        public ClientHttpManager Sender { get; private set; }

        public ClientHttpRequest Request { get; private set; }

        public float Progress { get; private set; }

        public Exception Error { get; private set; }

        internal HttpEventArgs(ClientHttpManager sender, ClientHttpRequest request, float progress)
        {
            Sender = sender;
            Request = request;
            Progress = progress;
        }

        internal HttpEventArgs(ClientHttpManager sender, ClientHttpRequest request, Exception error)
        {
            Sender = sender;
            Request = request;
            Error = error;
        }
    }

    /// <summary>
    /// 表示一個 HTTP manager，提供 Unity-based 的 HTTP 基礎通訊實作。
    /// </summary>
    public class ClientHttpManager
    {
        static int requestSerialNum;
        readonly PriorityQueue<ClientHttpRequest> queue = new PriorityQueue<ClientHttpRequest>();
        readonly Dictionary<string, string> commonRequestHeaders = new Dictionary<string, string>();

        public int RequestQueueCount
        {
            get { return queue.Count; }
        }

        public int SendingRequestCount { get; private set; }
        public int TotalRequestCount => RequestQueueCount + SendingRequestCount;

        public Func<IEnumerable<KeyValuePair<string, string>>> RequestCustomHeaders { get; set; }

        public void SetCommonRequestHeader(string name, string value)
        {
            commonRequestHeaders[name] = value;
        }

        public void SetCommonRequestHeaders(IEnumerable<KeyValuePair<string, string>> commonRequestHeaders)
        {
            if (commonRequestHeaders == null)
            {
                return;
            }

            foreach (var pair in commonRequestHeaders)
            {
                this.commonRequestHeaders[pair.Key] = pair.Value;
            }
        }

        static void SetRequestHeadersToWebRequest(UnityWebRequest webRequest, IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }
        }

        public ClientHttpRequest Request(ClientHttpRequestInfo requestInfo)
        {
            string uri = BuildUri(requestInfo.Host, requestInfo.Path, requestInfo.Query);
            DownloadHandlerBuffer uDownloadHandler = new DownloadHandlerBuffer();
            UnityWebRequest uWebRequest = new UnityWebRequest(uri, requestInfo.HttpMethod)
            {
                downloadHandler = uDownloadHandler,
            };

            if (requestInfo.RequestData != null && requestInfo.RequestData.Length > 0)
            {
                uWebRequest.uploadHandler = new UploadHandlerRaw(requestInfo.RequestData)
                {
                    contentType = requestInfo.ContentType
                };
            }
            int serial = Interlocked.Increment(ref requestSerialNum);
            var httpRequest = new ClientHttpRequest(serial, requestInfo, uWebRequest);
            SetRequestHeadersToWebRequest(uWebRequest, commonRequestHeaders);
            SetRequestHeadersToWebRequest(uWebRequest, requestInfo.ExtraHeaders);
            SetRequestHeadersToWebRequest(uWebRequest, RequestCustomHeaders.Raise());

            queue.Enqueue(httpRequest, 0);
            UniTask doTask = DoRequest();

            return httpRequest;
        }

        private async UniTask DoRequest()
        {
            if (queue.Count == 0)
            {
                return;
            }

            ClientHttpRequest httpRequest = queue.Dequeue();
            UnityWebRequest uWebRequest = httpRequest.WebRequest;
            UnityWebRequestAsyncOperation requestAyncOp = uWebRequest.SendWebRequest();
            float progress = -1F;

            while (!requestAyncOp.isDone)
            {
                if (progress < requestAyncOp.progress)
                {
                    progress = requestAyncOp.progress;
                    OnProgressUpdated(httpRequest, progress);
                }

                try
                {
                    httpRequest.OnProgressUpdated.Invoke(httpRequest, requestAyncOp.progress);
                }
                catch (Exception e)
                {
                    //logger.Error(e);
                }
                await UniTask.Yield();
            }

            progress = requestAyncOp.progress;
            OnProgressUpdated(httpRequest, progress);

            if (httpRequest.OnResponded != null)
            {
                //try
               // {
                    httpRequest.OnResponded.Invoke(httpRequest);
               // }
               // catch (Exception e)
                //{
                    //logger.Error(e, "Catch an exception from OnResponded. URI={0}", httpRequest.Uri);
                //}
            }

            OnResponded(httpRequest);
        }

        void OnResponded(ClientHttpRequest request)
        {
            try
            {
                //responded.OnNext(new HttpEventArgs(this, request, 1F));
            }
            catch (Exception e)
            {
                //logger.Error(e);
            }
        }

        void OnProgressUpdated(ClientHttpRequest request, float progress)
        {
            try
            {
                //logger.Debug("Sending \"{0}\"...{1:P2}", request.Uri, progress);
                //try
                //{
                    request.OnProgressUpdated.Invoke(request, progress);
                //}
               // catch (Exception e)
                //{
                    //logger.Error(e, " at request.OnProgressUpdate.Raise()");
                //}
                //progressUpdated.OnNext(new HttpEventArgs(this, request, progress));
            }
            catch (Exception e)
            {
                //logger.Error(e);
            }
        }

        void OnErrorOccurred(ClientHttpRequest request, Exception error)
        {
            try
            {
                //systemErrorOccurred.OnNext(new HttpEventArgs(this, request, error));
            }
            catch (Exception e)
            {
                //logger.Error(e);
            }
        }

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
                host = host.Remove(host.Length - 1, 1);
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