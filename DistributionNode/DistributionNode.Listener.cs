namespace DistributionNode
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal sealed class Listener
    {
        private IPEndPoint endPoint;
        private LogHandler log;
        private DataHandler handleData;

        public Listener(int port, DataHandler dataHandler, LogHandler logHandler)
        {
            this.endPoint = new IPEndPoint(IPAddress.Any, port);
            this.log = logHandler;
            this.handleData = dataHandler;
        }

        public delegate void LogHandler(string msg, int severity);

        public delegate string DataHandler(string data);

        public async void StartListener()
        {
            var listener = new TcpListener(this.endPoint);
            listener.Start();
            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    this.HandleClient(client);
                }
                catch (SocketException e)
                {
                    this.log(e.Message, 2);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            byte[] bytes = new byte[DistributionCommon.Constants.DistributionNode_Listener_StreamSize];
            string data = null;
            var stream = client.GetStream();

            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(this.handleData(data));
                stream.Write(msg, 0, msg.Length);
            }
        }
    }
}
