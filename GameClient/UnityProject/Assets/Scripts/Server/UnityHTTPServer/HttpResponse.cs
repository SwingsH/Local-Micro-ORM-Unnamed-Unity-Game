using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityHTTPServer
{
    public class HttpResponse : BaseHeader
    {
        public string StatusCode { get; set; }

        public string Protocols { get; set; }

        public string ProtocolsVersion { get; set; }

        public byte[] Content { get; private set; }

        private Stream handler;

        public ILogger Logger { get; set; }

        public HttpResponse(Stream stream)
        {
            this.handler = stream;
            this.Headers = new Dictionary<string, string>();
        }

        public HttpResponse SetContent(byte[] content, Encoding encoding = null)
        {
            this.Content = content;
            this.Encoding = encoding != null ? encoding : Encoding.UTF8;
            this.Content_Length = content.Length.ToString();
            return this;
        }

        public HttpResponse SetContent(string content, Encoding encoding = null)
        {
            encoding = encoding != null ? encoding : Encoding.UTF8;
            return SetContent(encoding.GetBytes(content), encoding);
        }

        public Stream GetResponseStream()
        {
            return this.handler;
        }

        public string GetHeader(ResponseHeaders header)
        {
            return GetHeaderByKey(header);
        }

        public string GetHeader(string fieldName)
        {
            return GetHeaderByKey(fieldName);
        }

        public void SetHeader(ResponseHeaders header, string value)
        {
            SetHeaderByKey(header, value);
        }

        public void SetHeader(string fieldName, string value)
        {
            SetHeaderByKey(fieldName, value);
        }

        protected string BuildHeader()
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(StatusCode))
                builder.Append("HTTP/1.1 " + StatusCode + "\r\n");

            if (!string.IsNullOrEmpty(this.Content_Type))
                builder.AppendLine("Content-Type:" + this.Content_Type);
            return builder.ToString();
        }

        public void Send()
        {
            if (!handler.CanWrite) return;

            try
            {
                var header = BuildHeader();
                byte[] headerBytes = this.Encoding.GetBytes(header);
                handler.Write(headerBytes, 0, headerBytes.Length);

                byte[] lineBytes = this.Encoding.GetBytes(System.Environment.NewLine);
                handler.Write(lineBytes, 0, lineBytes.Length);

                handler.Write(Content, 0, Content.Length);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
            finally
            {
                handler.Close();
            }
        }

        private void Log(object message)
        {
            if (Logger != null)
                Logger.Log(message);
        }
    }
}
