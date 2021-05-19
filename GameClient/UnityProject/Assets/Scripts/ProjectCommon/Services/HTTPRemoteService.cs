using System;
using UnityHTTPServer;

public class HTTPRemoteService
{

    public ClientHttpRequest Call(
        string hostId,
        string api,
        string query,
        string httpMethod,
        string contentType,
        byte[] data,
        Action<ClientHttpRequest> onResponded){

        var requestInfo = new ClientHttpRequestInfo
        {
            Host = string.Empty,
            Path = api,
            Query = query,
            HttpMethod = httpMethod,
            ContentType = contentType,
            RequestData = data,
            OnResponded = onResponded
        };
        //return HttpManager.Request(requestInfo);
        return null;
    }
}
