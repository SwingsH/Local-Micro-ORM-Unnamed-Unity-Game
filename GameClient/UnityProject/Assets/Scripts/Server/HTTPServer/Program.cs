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

namespace UnityHTTPServer
{
    public class Program : MonoBehaviour
    {
        Thread serverThread;
        void Start()
        {
            ExampleServer server = new ExampleServer("127.0.0.1", 4050);
            server.SetRoot(@"Z:\unitywww\");
            server.Logger = new ConsoleLogger();
            serverThread = new Thread( new ThreadStart(server.Start));
            serverThread.Start();
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
