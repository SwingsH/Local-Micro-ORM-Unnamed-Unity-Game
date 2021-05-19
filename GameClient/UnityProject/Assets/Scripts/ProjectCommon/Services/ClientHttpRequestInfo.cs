using System;
using System.Collections.Generic;
using UnityHTTPServer;

    public class ClientHttpRequestInfo
    {
        public string Host { get; set; }
        public string Path { get; set; }
        public string HttpMethod { get; set; }
        public string ContentType { get; set; }
        public byte[] RequestData { get; set; }
        public int MaxRetryCount { get; set; }
        public Action<ClientHttpRequest, float> OnProgressUpdated { get; set; }
        public Action<ClientHttpRequest> OnResponded { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ExtraHeaders { get; set; }
        public string Query { get; set; }
    }
