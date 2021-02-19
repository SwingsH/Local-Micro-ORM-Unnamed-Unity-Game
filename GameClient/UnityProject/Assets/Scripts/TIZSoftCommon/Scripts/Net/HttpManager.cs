using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TIZSoft.Extensions;
using TIZSoft.Log;
using TIZSoft.Utils;
using UniRx;
using UnityEngine.Networking;

namespace TIZSoft.Net
{
    /// <summary>
    /// 表示一個 HTTP 事件。
    /// </summary>
    public class HttpEventArgs
    {
        /// <summary>
        /// 取得發送此事件的來源。
        /// </summary>
        public HttpManager Sender { get; private set; }

        /// <summary>
        /// 取得此事件的 HTTP request。
        /// </summary>
        public HttpRequest Request { get; private set; }

        /// <summary>
        /// 取得觸發此事件時的通訊進度快照。
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 取得錯誤資訊。
        /// </summary>
        public Exception Error { get; private set; }

        internal HttpEventArgs(HttpManager sender, HttpRequest request, float progress)
        {
            Sender = sender;
            Request = request;
            Progress = progress;
        }

        internal HttpEventArgs(HttpManager sender, HttpRequest request, Exception error)
        {
            Sender = sender;
            Request = request;
            Error = error;
        }
    }

    /// <summary>
    /// 表示一個 HTTP manager，提供 Unity-based 的 HTTP 基礎通訊實作。
    /// </summary>
    public class HttpManager
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<HttpManager>();

        /// <summary>
        /// The global sequence identifier. 用來記錄 client 端 HTTP 通訊流水號。
        /// </summary>
        static int globalSequenceId;

        readonly Queue<HttpRequest> requestQueue = new Queue<HttpRequest>();
        readonly Dictionary<string, string> commonRequestHeaders = new Dictionary<string, string>();

        readonly Subject<HttpEventArgs> requesting = new Subject<HttpEventArgs>();
        readonly Subject<HttpEventArgs> responded = new Subject<HttpEventArgs>();
        readonly Subject<HttpEventArgs> progressUpdated = new Subject<HttpEventArgs>();
        readonly Subject<HttpEventArgs> systemErrorOccurred = new Subject<HttpEventArgs>();
        
        /// <summary>
        /// [Event] 發生於有新的 HTTP request 建立時。
        /// </summary>
        public IObservable<HttpEventArgs> Requesting
        {
            get { return requesting.AsObservable(); }
        }

        /// <summary>
        /// [Event] 發生於有 HTTP request 收到 response 時。
        /// </summary>
        public IObservable<HttpEventArgs> Responded
        {
            get { return responded.AsObservable(); }
        }

        /// <summary>
        /// [Event] 發生於有 HTTP request 通訊進度更新時。
        /// </summary>
        public IObservable<HttpEventArgs> ProgressUpdated
        {
            get { return progressUpdated.AsObservable(); }
        }

        /// <summary>
        /// [Event] 發生於 HTTP 通訊時遇到 system error。
        /// </summary>
        public IObservable<HttpEventArgs> SystemErrorOccurred
        {
            get { return systemErrorOccurred.AsObservable(); }
        }

        /// <summary>
        /// 取得正在佇列中的 request 數量。
        /// </summary>
        public int RequestQueueCount
        {
            get { return requestQueue.Count; }
        }

        /// <summary>
        /// 取得正在通訊中的 request 數量。
        /// </summary>
        public int SendingRequestCount { get; private set; }

        /// <summary>
        /// 取得所有 request 的數量，等同於 <see cref="RequestQueueCount"/> + <see cref="SendingRequestCount"/>。
        /// </summary>
        public int TotalRequestCount
        {
            get { return RequestQueueCount + SendingRequestCount; }
        }

        /// <summary>
        /// 取得或設定每次 HTTP request 要執行的 Headers 產生方法，此方法產生的 Headers 將會添加到 HTTP request Headers。
        /// </summary>
        public Func<IEnumerable<KeyValuePair<string, string>>> RequestHeadersCreator { get; set; }

        /// <summary>
        /// 設定每次 HTTP request 都需要設定的 Header。
        /// </summary>
        /// <param name="name">Header key.</param>
        /// <param name="value">Header value.</param>
        /// <exception cref="ArgumentNullException">
        ///     參數 <paramref name="name"/> 為 null。
        /// </exception>
        public void SetCommonRequestHeader(string name, string value)
        {
            ExceptionUtils.VerifyArgumentNull(name, "name");
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

        /// <summary>
        /// 根據給定的參數內容來發送一個 HTTP request。
        /// </summary>
        /// <returns>The HTTP request.</returns>
        /// <param name="httpRequestInfo">HTTP request info.</param>
        /// <exception cref="ArgumentException">
        ///     參數 <paramref name="httpRequestInfo"/> 的 Host 或 HttpMethod 屬性為 null 或空字串。
        /// </exception>
        public HttpRequest Request(HttpRequestInfo httpRequestInfo)
        {
            VerifyHttpRequestInfo(httpRequestInfo);
            
            // 建立 request
            var uri = UriUtils.BuildUri(httpRequestInfo.Host, httpRequestInfo.Path, httpRequestInfo.Query);
            var downloadHandler = new DownloadHandlerBuffer();
            var webRequest = new UnityWebRequest(
                uri,
                httpRequestInfo.HttpMethod)
                {
                    downloadHandler = downloadHandler
                };
            if (httpRequestInfo.RequestData != null && httpRequestInfo.RequestData.Length > 0)
            {
                webRequest.uploadHandler = new UploadHandlerRaw(httpRequestInfo.RequestData)
                {
                    contentType = httpRequestInfo.ContentType
                };
            } 
            var sequenceId = Interlocked.Increment(ref globalSequenceId);
            var httpRequest = new HttpRequest(sequenceId, httpRequestInfo, webRequest);
            SetRequestHeadersToWebRequest(webRequest, commonRequestHeaders);
            SetRequestHeadersToWebRequest(webRequest, httpRequestInfo.ExtraHeaders);
            SetRequestHeadersToWebRequest(webRequest, RequestHeadersCreator.Raise());
            OnRequesting(httpRequest);

            // 將請求推入隊列，實際上的 request 是由 RequestCore() 執行
            requestQueue.Enqueue(httpRequest);
            Observable.FromMicroCoroutine(RequestCore)
                .DoOnError(e =>
                {
                    logger.Error(e, "An error occurred when HTTP request.");
                    OnErrorOccurred(httpRequest, e);
                })
                .Subscribe();

            return httpRequest;
        }

        static void VerifyHttpRequestInfo(HttpRequestInfo httpRequestInfo)
        {
            ExceptionUtils.VerifyArgumentNullOrEmpty(httpRequestInfo.Host, "httpRequestInfo.Host");
            ExceptionUtils.VerifyArgumentNullOrEmpty(httpRequestInfo.HttpMethod, "httpRequestInfo.HttpMethod");
        }
        
        IEnumerator RequestCore()
        {
            // Request queue 若沒東西，就直接結束。
            if (requestQueue.Count == 0)
            {
                yield break;
            }

            // 把 request 抓出來，開始傳
            var httpRequest = requestQueue.Dequeue();
            var webRequest = httpRequest.WebRequest;
            var sendAsync = webRequest.Send();
            var progress = -1F;
            while (!sendAsync.isDone)
            {
                if (progress < sendAsync.progress)
                {
                    progress = sendAsync.progress;
                    OnProgressUpdated(httpRequest, progress);
                }

                // 不能保證 client-side 的 callback 有處理錯誤
                // 必須要捕捉 exception，否則 coroutine 會出錯
                try
                {
                    httpRequest.OnProgressUpdated.Raise(httpRequest, sendAsync.progress);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
                yield return null;
            }
            progress = sendAsync.progress;
            OnProgressUpdated(httpRequest, progress);

            // 發生錯誤時仍必須 invoke OnResponded，必須讓 client-side 程式知道有錯誤發生
            // 這裡是 system error，不是 HTTP error
            var errorMessage = new StringBuilder();
            if (webRequest.isNetworkError)
            {
                logger.Error(webRequest.error);
                errorMessage.AppendLine("WebRequest System Error:");
                errorMessage.AppendLine(webRequest.error);
            }

            if (httpRequest.OnResponded != null)
            {
                // 不能保證 client-side 的 callback 有處理錯誤
                // 必須要捕捉 exception，否則 coroutine 會出錯
                try
                {
                    httpRequest.OnResponded.Raise(httpRequest);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Catch an exception from OnResponded. URI={0}", httpRequest.Uri);
                    errorMessage.AppendLine("httpRequest.OnResponded Error:");
                    errorMessage.AppendLine(e.ToString());
                }
            }

            OnResponded(httpRequest);

            if (errorMessage.Length > 0)
            {
                OnErrorOccurred(httpRequest, new Exception(errorMessage.ToString()));
            }
        }

        void OnRequesting(HttpRequest request)
        {
            try
            {
                requesting.OnNext(new HttpEventArgs(this, request, 0F));
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        void OnResponded(HttpRequest request)
        {
            try
            {
                responded.OnNext(new HttpEventArgs(this, request, 1F));
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        void OnProgressUpdated(HttpRequest request, float progress)
        {
            try
            {
                logger.Debug("Sending \"{0}\"...{1:P2}", request.Uri, progress);
                try
                {
                    request.OnProgressUpdated.Raise(request, progress);
                }
                catch (Exception e)
                {
                    logger.Error(e, " at request.OnProgressUpdate.Raise()");
                }
                progressUpdated.OnNext(new HttpEventArgs(this, request, progress));
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        void OnErrorOccurred(HttpRequest request, Exception error)
        {
            try
            {
                systemErrorOccurred.OnNext(new HttpEventArgs(this, request, error));
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
