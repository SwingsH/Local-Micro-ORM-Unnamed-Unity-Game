using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
//using System.Runtime.InteropServices;

namespace SimpleHTTPServer
{
    /// <summary>
    /// Original : HttpServerLib in Github
    /// </summary>
    public class HttpServer : IServer
    {
        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }
        public string ServerRoot { get; private set; }
        public bool IsRunning { get; private set; }
        public Protocols Protocol { get; private set; }
        private TcpListener serverListener;
        public ILogger Logger { get; set; }
        private X509Certificate serverCertificate = null;
        private HttpServer(IPAddress ipAddress, int port, string root)
        {
            this.ServerIP = ipAddress.ToString();
            this.ServerPort = port;
            if (!Directory.Exists(root))
                this.ServerRoot = AppDomain.CurrentDomain.BaseDirectory;

            this.ServerRoot = root;
        }

        public HttpServer(string ipAddress, int port, string root) :
            this(IPAddress.Parse(ipAddress), port, root)
        { }

        public HttpServer(string ipAddress, int port) :
            this(IPAddress.Parse(ipAddress), port, AppDomain.CurrentDomain.BaseDirectory)
        { }

        public HttpServer(int port, string root) :
            this(IPAddress.Loopback, port, root)
        { }

        public HttpServer(int port) :
            this(IPAddress.Loopback, port, AppDomain.CurrentDomain.BaseDirectory)
        { }

        public HttpServer(string ip) :
            this(IPAddress.Parse(ip), 80, AppDomain.CurrentDomain.BaseDirectory)
        { }

        #region 公开方法

        public void Start()
        {
            if (IsRunning) return;
            this.serverListener = new TcpListener(IPAddress.Parse(ServerIP), ServerPort);
            this.Protocol = serverCertificate == null ? Protocols.Http : Protocols.Https;
            this.IsRunning = true;
            this.serverListener.Start();
            this.Log(string.Format("Sever is running at {0}://{1}:{2}", Protocol.ToString().ToLower(), ServerIP, ServerPort));

            try
            {
                while (IsRunning)
                {
                    TcpClient client = serverListener.AcceptTcpClient(); // TODO: run in background thread
                    Thread requestThread = new Thread(() => { ProcessRequest(client); });
                    requestThread.Start();
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        }


        public HttpServer SetSSL(string certificate)
        {
            return SetSSL(X509Certificate.CreateFromCertFile(certificate));
        }


        public HttpServer SetSSL(X509Certificate certifiate)
        {
            this.serverCertificate = certifiate;
            return this;
        }

        public void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
            serverListener.Stop();
        }

        public HttpServer SetRoot(string root)
        {
            if (!Directory.Exists(root))
                this.ServerRoot = AppDomain.CurrentDomain.BaseDirectory;

            this.ServerRoot = root;
            return this;
        }

        public string GetRoot()
        {
            return this.ServerRoot;
        }

        public HttpServer SetPort(int port)
        {
            this.ServerPort = port;
            return this;
        }


        #endregion

        #region 内部方法

        private void ProcessRequest(TcpClient handler)
        {
            Stream clientStream = handler.GetStream();

            if (serverCertificate != null) clientStream = ProcessSSL(clientStream);
            if (clientStream == null) return;

            HttpRequest request = new HttpRequest(clientStream);
            request.Logger = Logger;

            HttpResponse response = new HttpResponse(clientStream);
            response.Logger = Logger;

            switch (request.Method)
            {
                case "GET":
                    OnGet(request, response);
                    break;
                case "POST":
                    OnPost(request, response);
                    break;
                default:
                    OnDefault(request, response);
                    break;
            }
        }

        private Stream ProcessSSL(Stream clientStream)
        {
            try
            {
                SslStream sslStream = new SslStream(clientStream);
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
                sslStream.ReadTimeout = 10000;
                sslStream.WriteTimeout = 10000;
                return sslStream;
            }
            catch (Exception e)
            {
                Log(e.Message);
                clientStream.Close();
            }

            return null;
        }

        protected void Log(object message)
        {
            if (Logger != null) Logger.Log(message);
        }

        #endregion

        #region 虚方法

        public virtual void OnGet(HttpRequest request, HttpResponse response)
        {

        }
        public virtual void OnPost(HttpRequest request, HttpResponse response)
        {

        }
        public virtual void OnDefault(HttpRequest request, HttpResponse response)
        {

        }

        #endregion
    }
}
