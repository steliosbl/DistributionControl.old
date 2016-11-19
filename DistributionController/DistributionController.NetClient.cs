namespace DistributionController
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal sealed class NetClient
    {
        private IPEndPoint remoteEP;
        
        public NetClient(IPAddress address, int port, EndPointUnreachableHandler unreachableHandler)
        {
            this.remoteEP = new IPEndPoint(address, port);
            this.EndPointUnreachable += unreachableHandler;
        }

        public delegate void EndPointUnreachableHandler();

        public event EndPointUnreachableHandler EndPointUnreachable;

        public string Send(string message)
        {
            try
            {
                var client = new TcpClient(this.remoteEP);
                var stream = client.GetStream();

                var data = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                data = new byte[DistributionCommon.Constants.Communication.StreamSize];
                int bytes = stream.Read(data, 0, data.Length);
                string response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                stream.Close();
                client.Close();

                return response;
            }
            catch (SocketException)
            {
                this.OnEndPointUnreachable();
                return null;
            }
        }

        private void OnEndPointUnreachable()
        {
            if (this.EndPointUnreachable != null)
            {
                this.EndPointUnreachable();
            }
        }
    }
}
