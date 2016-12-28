namespace DistributionController.API
{
    using System;
    using System.Net;
    using System.Net.Http;

    internal sealed class NetListener
    {
        private int port;
        private HttpListener listener;
        private GetRequestHandler getHandler;

        public NetListener(int port, string[] pages, GetRequestHandler getHandler)
        {
            this.port = port;
            this.listener = new HttpListener();
            this.listener.Prefixes.Add("http://localhost:" + port.ToString() + "/");
            foreach (string page in pages)
            {
                this.listener.Prefixes.Add("http://localhost:" + port.ToString() + "/" + page + "/");
            }

            this.getHandler = getHandler;
        }

        public delegate string GetRequestHandler(string subdirectory);

        public void Start()
        {
            this.listener.Start();
            while (this.listener.IsListening)
            {
                var context = this.listener.GetContext();
                var request = context.Request;

                if (request.HttpMethod == "GET")
                {
                    string subdir = request.Url.Segments[1].ToString().Replace("/", string.Empty);
                    if (subdir != "favicon.ico")
                    {
                        byte[] b = System.Text.Encoding.UTF8.GetBytes(this.getHandler(subdir));
                        context.Response.StatusCode = 200;
                        context.Response.KeepAlive = false;
                        context.Response.AppendHeader("Access-Control-Allow-Origin", "http://localhost:8000");
                        context.Response.ContentLength64 = b.Length;
                        context.Response.OutputStream.Write(b, 0, b.Length);
                        context.Response.Close();
                    }
                }
            }
        }
    }
}
