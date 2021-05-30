using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityHTTPServer;
using System.IO;
using TIZSoft.UnityHTTP;
using Newtonsoft.Json.Linq;
using TIZSoft.Utils.Log;
using TIZSoft.UnknownGame.Common.API;

namespace HttpServer
{
    public delegate JObject ProtocolMapping(string jsonBody);

    public class ApiMetaData
    {
        public string ApiUrl;
        public Type ApiRequestClass;
        public ProtocolMapping ProtocolMethod;
    }
    public class GameApiServer : UnityHTTPServer.HttpServer
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<GameApiServer>();

        protected Dictionary<string, ApiMetaData> protocolMapping;
        public GameApiServer(string ipAddress, int port)
            : base(ipAddress, port)
        {

        }

        public void Initialize()
        {
            InitApiEntries();

            RegisterProtocol(typeof(UserRequest),           ApiUser.GetUserInfo);
            RegisterProtocol(typeof(UserNameChangeRequest), ApiUser.ChangeUserName);
            RegisterProtocol(typeof(UserTeamChangeRequest), ApiUser.ChangeUserTeamName);
        }

        private void InitApiEntries()
        {
            protocolMapping = new Dictionary<string, ApiMetaData>();

            List<Type> apiClasses = TIZSoft.Extensions.TypeExtensions.GetTypesHasAttribute<EntryPointAttribute>();
            ApiMetaData meta = default;
            foreach (var api in apiClasses)
            {
                object[] attrs = api.GetCustomAttributes(typeof(EntryPointAttribute), false);
                EntryPointAttribute entry = attrs[0] as EntryPointAttribute;
                string tokenUrl = NormalizeUrlToToken(entry.partialUrl);
                //logger.Debug("NormalizeUrlToToken: "+ tokenUrl);
                meta = new ApiMetaData {
                    ApiUrl = tokenUrl,
                    ApiRequestClass = api,
                    ProtocolMethod = default
                };

                protocolMapping.Add(tokenUrl, meta);
            }
        }

        protected void RegisterProtocol(Type apiClass, ProtocolMapping protocol)
        {
            ApiMetaData myMeta = protocolMapping.Where(meta => meta.Value.ApiRequestClass == apiClass).FirstOrDefault().Value;
            if(myMeta==null)
            {
                logger.Log(LogLevel.Error, " RegisterProtocol failed. " + apiClass.Name);
                return;
            }
            myMeta.ProtocolMethod = protocol;
            //[user/my] [UserRequest] [GetUserInfo] 
            ///logger.Log(LogLevel.Debug, string.Format("RegProto : [{0}] [{1}] [{2}] ", myMeta.ApiUrl, myMeta.ApiRequestClass.Name, myMeta.ProtocolMethod.Method.Name));
        }

        protected string NormalizeUrlToToken(string Url)
        {
            char[] charsToTrim = { ' ', '\'' , '/'};
            Url = Url.Trim(charsToTrim).Replace('\'', '/').Replace("//", "/");
            return Url;
        }

        public override void OnPost(ServerSideHttpRequest request, HttpResponse response)
        {
            //string data = request.Params == null ? "" : string.Join(";", request.Params.Select(x => x.Key + "=" + x.Value).ToArray());

            //string content = string.Format("post response :{0}", data);

            //response.SetContent(content);
            //response.Content_Encoding = "utf-8";
            //response.StatusCode = "200";
            //response.Content_Type = "text/html; charset=UTF-8";
            //response.Headers["Server"] = "ExampleServer";

            response.Send();
        }

        public override void OnGet(ServerSideHttpRequest request, HttpResponse response)
        {
            string jsonStr = request.Body;
            string tokenUrl = NormalizeUrlToToken(request.URL);
            //logger.Debug(string.Format("OnGet [{0}] ", tokenUrl));

            if (protocolMapping.ContainsKey(tokenUrl))
            {
                if (protocolMapping[tokenUrl].ProtocolMethod == null)
                {
                    logger.Log(LogLevel.Error, string.Format("[{0}] not defined a api method. ", tokenUrl));
                    return;
                }
                //logger.Debug(string.Format("Try to call method [{0}] [{1}]", tokenUrl, protocolMapping[tokenUrl].ProtocolMethod.Method.Name));
                protocolMapping[tokenUrl].ProtocolMethod.Invoke(jsonStr);
                //response.Body = " just test a response.Body from game server !";
                response = response.FromText(" just test a response.Body from game server !");
                UnityEngine.Debug.Log(response.Body);
                //response.Content_Type = "text/html; charset=UTF-8";
            }

            //// response forbidden 404
            //string requestURL = request.URL;
            //requestURL = requestURL.Replace("/", @"\").Replace("\\..", "").TrimStart('\\');
            //string requestFile = Path.Combine(ServerRoot, requestURL);

            //string extension = Path.GetExtension(requestFile);

            //if (extension != "")
            //{
            //    //response specific file
            //    response = response.FromFile(requestFile);
            //} 
            //else
            //{
            //    //response the directory list
            //    if (Directory.Exists(requestFile) && !File.Exists(requestFile + "\\index.html"))
            //    {
            //        requestFile = Path.Combine(ServerRoot, requestFile);
            //        var content = ListDirectory(requestFile, requestURL);
            //        response = response.SetContent(content, Encoding.UTF8);
            //        response.Content_Type = "text/html; charset=UTF-8";
            //    } 
            //    else
            //    {
            //        //response index file
            //        requestFile = Path.Combine(requestFile, "index.html");
            //        response = response.FromFile(requestFile);
            //        response.Content_Type = "text/html; charset=UTF-8";
            //    }
            //}

            response.Send();
        }

        public override void OnDefault(ServerSideHttpRequest request, HttpResponse response)
        {

        }
    }
}
