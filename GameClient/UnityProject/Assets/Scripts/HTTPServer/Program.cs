using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SimpleHTTPServer;
using HttpServer;

using UnityEngine;

namespace SimpleHTTPServer
{
    public class Program : MonoBehaviour
    {
        void Start()
        {
            ExampleServer server = new ExampleServer("127.0.0.1", 4050);
            server.SetRoot(@"Z:\unitywww\");
            server.Logger = new ConsoleLogger();
            server.Start();
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
