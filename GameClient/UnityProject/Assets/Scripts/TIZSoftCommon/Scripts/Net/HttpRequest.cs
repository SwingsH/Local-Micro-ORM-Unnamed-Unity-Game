using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace TIZSoft.Net
{
    /// <summary>
    /// HTTP request.
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// Gets the http request info.
        /// </summary>
        /// <value>The http request info.</value>
        public HttpRequestInfo HttpRequestInfo { get; private set; }

        /// <summary>
        /// Gets the Unity web request.
        /// </summary>
        /// <value>The web request.</value>
        public UnityWebRequest WebRequest { get; private set; }

        /// <summary>
        /// Gets the sequence identifier.
        /// </summary>
        /// <value>The sequence identifier.</value>
        public int SequenceId { get; private set; }

        /// <summary>
        /// Gets the request timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get { return HttpRequestInfo.Host; } }

        /// <summary>
        /// Gets the path (Web API name).
        /// </summary>
        /// <value>The path.</value>
        public string Path { get { return HttpRequestInfo.Path; } }

        /// <summary>
        /// Gets the actual URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get { return WebRequest.url; } }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        /// <value>The http method.</value>
        public string HttpMethod { get { return WebRequest.method; } }

        /// <summary>
        /// Gets the request data (body).
        /// </summary>
        /// <value>The request data.</value>
        public byte[] RequestData { get { return HttpRequestInfo.RequestData; } }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>The http status code.</value>
        public int HttpStatusCode { get { return (int)WebRequest.responseCode; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Tizsoft.Violin.Net.HttpRequest"/> has system error.
        /// </summary>
        /// <value><c>true</c> if has error; otherwise, <c>false</c>.</value>
        public bool HasSystemError { get { return WebRequest.isNetworkError; } }

        /// <summary>
        /// Gets the HTTP system error.
        /// </summary>
        /// <value>The error.</value>
        public string SystemError { get { return WebRequest.error; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Tizsoft.Violin.Net.HttpRequest"/> is responded.
        /// </summary>
        /// <value><c>true</c> if is responded; otherwise, <c>false</c>.</value>
        public bool IsResponded { get { return WebRequest.isDone; } }

        /// <summary>
        /// Gets the response headers.
        /// </summary>
        /// <value>The response headers.</value>
        public IDictionary<string, string> ResponseHeaders { get { return WebRequest.GetResponseHeaders(); } }

        /// <summary>
        /// Gets the response data (body).
        /// </summary>
        /// <value>The response data.</value>
        public byte[] ResponseData { get { return WebRequest.downloadHandler.data; } }

        public int CurrentRetryCount { get; private set; }

        public int MaxRetryCount { get { return HttpRequestInfo.MaxRetryCount; } }

        public bool CanRetry { get { return MaxRetryCount == -1 || MaxRetryCount > CurrentRetryCount; } }

        public Action<HttpRequest, float> OnProgressUpdated { get { return HttpRequestInfo.OnProgressUpdated; } }

        public Action<HttpRequest> OnResponded { get { return HttpRequestInfo.OnResponded; } }

        public HttpRequest(int sequenceId, HttpRequestInfo httpRequestInfo, UnityWebRequest webRequest)
        {
            SequenceId = sequenceId;
            HttpRequestInfo = httpRequestInfo;
            WebRequest = webRequest;
        }
        
        public override string ToString()
        {
            return string.Format(
                "[HttpRequest: SequenceId={0}, Timestamp={1}, Uri={2}, HttpMethod={3}, CurrentRetryCount={4}, MaxRetryCount={5}]",
                SequenceId, Timestamp, Uri, HttpMethod, CurrentRetryCount, MaxRetryCount);
        }

        /// <summary>
        /// 從 response data 取得 UTF-8 字串。
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return GetText(Encoding.UTF8);
        }

        /// <summary>
        /// 從 response data 取得指定編碼的字串。
        /// </summary>
        /// <returns></returns>
        public string GetText(Encoding encoding)
        {
            return encoding.GetString(ResponseData);
        }
    }
}
