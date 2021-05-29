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

    public class GameWebServer : UnityHTTPServer.HttpServer
    {
        static readonly Logger logger = LogManager.Default.FindOrCreateLogger<GameWebServer>();

        protected Dictionary<string, ApiMetaData> protocolMapping;
        public GameWebServer(string ipAddress, int port)
            : base(ipAddress, port)
        {

        }

        public override void OnPost(ServerSideHttpRequest request, HttpResponse response)
        {
            string data = request.Params == null ? "" : string.Join(";", request.Params.Select(x => x.Key + "=" + x.Value).ToArray());

            string content = string.Format("这是通过Post方式返回的数据:{0}", data);

            response.SetContent(content);
            response.Content_Encoding = "utf-8";
            response.StatusCode = "200";
            response.Content_Type = "text/html; charset=UTF-8";
            response.Headers["Server"] = "ExampleServer";

            response.Send();
        }

        public override void OnGet(ServerSideHttpRequest request, HttpResponse response)
        {
            //================= Under is Web Server about =================
            // url type 1: "http://localhost:4050/assets/styles/style.css" response specific file "/assets/styles/style.css"
            // url type 2: "http://localhost:4050/assets/styles/" pages. response index file "/assets/styles/style.index"
            // url type 3: response the directory list.
            // response forbidden 404
            string requestURL = request.URL;
            requestURL = requestURL.Replace("/", @"\").Replace("\\..", "").TrimStart('\\');
            string requestFile = Path.Combine(ServerRoot, requestURL);

            string extension = Path.GetExtension(requestFile);

            if (extension != "")
            {
                //response specific file
                response = response.FromFile(requestFile);
            }
            else
            {
                //response the directory list
                if (Directory.Exists(requestFile) && !File.Exists(requestFile + "\\index.html"))
                {
                    requestFile = Path.Combine(ServerRoot, requestFile);
                    var content = ListDirectory(requestFile, requestURL);
                    response = response.SetContent(content, Encoding.UTF8);
                    response.Content_Type = "text/html; charset=UTF-8";
                }
                else
                {
                    //response index file
                    requestFile = Path.Combine(requestFile, "index.html");
                    response = response.FromFile(requestFile);
                    response.Content_Type = "text/html; charset=UTF-8";
                }
            }
            response.Send();
        }

        public override void OnDefault(ServerSideHttpRequest request, HttpResponse response)
        {

        }

        private string ConvertPath(string[] urls)
        {
            string html = string.Empty;
            int length = ServerRoot.Length;
            foreach (var url in urls)
            {
                var s = url.StartsWith("..") ? url : url.Substring(length).TrimEnd('\\');
                html += String.Format("<li><a href=\"{0}\">{0}</a></li>", s);
            }

            return html;
        }

        private string ListDirectory(string requestDirectory, string requestURL)
        {
            //list subfolders
            var folders = requestURL.Length > 1 ? new string[] { "../" } : new string[] { };
            folders = folders.Concat(Directory.GetDirectories(requestDirectory)).ToArray();
            var foldersList = ConvertPath(folders);

            //list files
            var files = Directory.GetFiles(requestDirectory);
            var filesList = ConvertPath(files);

            //build HTML
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("<html><head><title>{0}</title></head>", requestDirectory));
            builder.Append(string.Format("<body><h1>{0}</h1><br/><ul>{1}{2}</ul></body></html>",
                 requestURL, filesList, foldersList));

            return builder.ToString();
        }
    }
}
