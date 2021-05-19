using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections.Generic;

public class ClientHttpRequest
{
    public ClientHttpRequestInfo HttpRequestInfo { get; private set; }
    public UnityWebRequest WebRequest { get; private set; }
    public int SerialID { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Host => HttpRequestInfo.Host;
    public string Path => HttpRequestInfo.Path;
    public string Uri { get { return WebRequest.url; } }
    public string HttpMethod { get { return WebRequest.method; } }
    public byte[] RequestData { get { return HttpRequestInfo.RequestData; } }
    public int HttpStatusCode { get { return (int)WebRequest.responseCode; } }
    public string SystemError { get { return WebRequest.error; } }
    public bool IsResponded { get { return WebRequest.isDone; } }
    public IDictionary<string, string> ResponseHeaders { get { return WebRequest.GetResponseHeaders(); } }
    public byte[] ResponseData { get { return WebRequest.downloadHandler.data; } }
    public int CurrentRetryCount { get; private set; }
    public int MaxRetryCount { get { return HttpRequestInfo.MaxRetryCount; } }
    public bool CanRetry { get { return MaxRetryCount == -1 || MaxRetryCount > CurrentRetryCount; } }
    public Action<ClientHttpRequest, float> OnProgressUpdated => HttpRequestInfo.OnProgressUpdated;
    public Action<ClientHttpRequest> OnResponded => HttpRequestInfo.OnResponded;
    public ClientHttpRequest(int serial, ClientHttpRequestInfo httpRequestInfo, UnityWebRequest webRequest)
    {
        SerialID = serial;
        HttpRequestInfo = httpRequestInfo;
        WebRequest = webRequest;
    }

    public override string ToString() => string.Format(
            "[HttpRequest: SequenceId={0}, Timestamp={1}, Uri={2}, HttpMethod={3}, CurrentRetryCount={4}, MaxRetryCount={5}]",
            SerialID, Timestamp, Uri, HttpMethod, CurrentRetryCount, MaxRetryCount);

    public string GetText() => GetText(Encoding.UTF8);
    public string GetText(Encoding encoding) => encoding.GetString(ResponseData);
}