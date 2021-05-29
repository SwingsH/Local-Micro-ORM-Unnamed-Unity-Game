using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityHTTPServer;
using HttpServer;
using UnityEngine;
using System.Threading;
using TIZSoft;

namespace UnityHTTPServer
{
    public class ServerMain : MonoBehaviour
    {
        const string GAME_SERVER_ROOT_PATH = "wwwroot";
        Thread serverThread;
        void Start()
        {
            GameHttpServer server = new GameHttpServer(Constants.GAME_SERVER_ENTRANCE_IPADDRESS, Constants.GAME_SERVER_ENTRANCE_PORT);
            server.Initialize();
            server.SetRoot(string.Format("{0}/{1}/", Application.dataPath, GAME_SERVER_ROOT_PATH));
            server.Logger = new ConsoleLogger();
            serverThread = new Thread( new ThreadStart(server.Start));
            serverThread.Start();

            Application.OpenURL("http://127.0.0.1:4050/");
        }

        void HTTPClient() // test conn
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:4050/");
            request.Method = "POST";
            var requestStream = request.GetRequestStream();
            var data = Encoding.UTF8.GetBytes("a=10&b=15");
            requestStream.Write(data, 0, data.Length);
            var response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();
            }
        }
    }
}
