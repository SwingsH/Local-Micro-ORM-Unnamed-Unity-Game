using UnityEngine.Networking;

namespace TIZSoft.Net
{
    /// <summary>
    /// 定義 HTTP 方法名稱。
    /// </summary>
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
}
