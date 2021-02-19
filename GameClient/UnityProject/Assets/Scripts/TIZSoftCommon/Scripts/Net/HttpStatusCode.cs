using System;

namespace TIZSoft.Net
{
    /// <summary>
    /// 定義 HTTP 狀態碼。
    /// </summary>
    /// <remarks>
    /// https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
    /// </remarks>
    public static class HttpStatusCode
    {
        #region 1xx Informational responses

        public const int Continue = 100;
        public const int SwitchingProtocols = 101;
        public const int Processing = 102;  // WebDAV; RFC 2518

        #endregion

        #region 2xx Success

        public const int Ok = 200;
        public const int Created = 201;
        public const int Accepted = 202;
        public const int NonAuthoritativeInformation = 203; // HTTP/1.1
        public const int NoContent = 204;
        public const int ResetContent = 205;
        public const int PartialContent = 206;  // RFC 7233
        public const int MultiStatus = 207;     // WebDAV; RFC 4918
        public const int AlreadyReported = 208; // WebDAV; RFC 5842
        public const int ImUsed = 226;          // RFC 3229

        #endregion

        #region 3xx Redirection

        public const int MultipleChoices = 300;
        public const int MovedPermanently = 301;
        public const int Found = 302;
        public const int SeeOther = 303;    // HTTP/1.1
        public const int NotModified = 304; // RFC 7232
        public const int UseProxy = 305;    // HTTP/1.1
        [Obsolete("No longer used. See https://tools.ietf.org/html/draft-cohen-http-305-306-responses-00")]
        public const int SwitchProxy = 306;
        public const int TemporaryRedirect = 307;   // HTTP/1.1
        public const int PermanentRedirect = 308;    // RFC 7538

        #endregion

        #region 4xx Client errors

        public const int BadRequest = 400;
        public const int Unauthorized = 401;    // RFC 7235
        public const int PaymentRequired = 402;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int MethodNotAllowed = 405;
        public const int NotAcceptable = 406;
        public const int ProxyAuthenticationRequired = 407; // RFC 7235
        public const int RequestTimeout = 408;
        public const int Conflict = 409;
        public const int Gone = 410;
        public const int LengthRequired = 411;
        public const int PreconditionFailed = 412;  // RFC 7232
        public const int PayloadTooLarge = 413;     // RFC 7231
        public const int UriTooLong = 414;          // RFC 7231
        public const int UnsupportedMediaType = 415;
        public const int RangeNotSatisfiable = 416; // RFC 7233
        public const int ExpectationFailed = 417;
        public const int ImATeapot = 418;           // RFC 2324. 這是 1998 年愚人節笑話，別真的去用。
        public const int MisdirectedRequest = 421;  // RFC 7540
        public const int UnprocessableEntity = 422; // WebDAV; RFC 4918
        public const int Locked = 423;              // WebDAV; RFC 4918
        public const int FailedDependency = 424;    // WebDAV; RFC 4918
        public const int UpgradeRequired = 426;
        public const int PreconditionRequired = 428;// RFC 6585
        public const int TooManyRequests = 429;     // RFC 6585
        public const int RequestHeaderFieldsTooLarge = 431; // RFC 6585
        public const int UnavailableForLegalReasons = 451;  // RFC 7725

        #endregion

        #region 5xx Server errors

        public const int InternalServerError = 500;
        public const int NotImplemented = 501;
        public const int BadGateway = 502;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
        public const int HttpVersionNotSupported = 505;
        public const int VariantAlsoNegotiates = 506;   // RFC 2295
        public const int InsufficientStorage = 507;     // WebDAV; RFC 4918
        public const int LoopDetected = 508;            // WebDAV: RFC 5842
        public const int NetExtended = 510;             // RFC 2774
        public const int NetworkAuthenticationRequired = 511;   // RFC 6585

        #endregion
    }
}