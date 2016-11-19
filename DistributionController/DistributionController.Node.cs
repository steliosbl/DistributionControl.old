namespace DistributionController
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Newtonsoft.Json;

    internal sealed class Node
    {
        public readonly DistributionCommon.Schematic.Node Schematic;
        private NetClient client;
        private Thread watchdog;

        public Node(DistributionCommon.Schematic.Node schematic, LostNodeHandler lostHandler, RecoveredNodeHandler recoveredHandler, int pingDelay)
        {
            this.Schematic = schematic;
            this.Reachable = false;
            this.client = new NetClient(this.Schematic.Address, this.Schematic.Port, this.OnLostNode);

            this.watchdog = new Thread(() => this.PingLoop(pingDelay));
            this.watchdog.Start();

            Thread.Sleep(pingDelay);
            if (this.Reachable)
            {
                if (this.Status())
                {
                    this.Reset();
                }

                this.Construct();
            }
            else
            {
                throw new InitializationException();
            }

            this.LostNode += lostHandler;
            this.RecoveredNode = recoveredHandler;
        }

        public delegate void LostNodeHandler();

        public delegate void RecoveredNodeHandler();

        public event LostNodeHandler LostNode;
        
        public event RecoveredNodeHandler RecoveredNode;

        public bool Reachable { get; private set; }
        
        public bool Assign(Job job)
        {
            var request = new DistributionCommon.Requests.Assign(job.Blueprint);
            var response = this.SendRequest<DistributionCommon.Responses.Assign>(request);
            if (response != default(DistributionCommon.Responses.Assign))
            {
                return response.Success;
            }

            return false;
        }

        public bool Construct()
        {
            var request = new DistributionCommon.Requests.Construct(this.Schematic);
            var response = this.SendRequest<DistributionCommon.Responses.Construct>(request);
            if (response != default(DistributionCommon.Responses.Construct))
            {
                return response.Success;
            }

            return false;
        }

        public bool Remove(int id)
        {
            var request = new DistributionCommon.Requests.Remove(id);
            var response = this.SendRequest<DistributionCommon.Responses.Remove>(request);
            if (response != default(DistributionCommon.Responses.Remove))
            {
                return response.Success;
            }

            return false;
        }

        public bool Reset()
        {
            var request = new DistributionCommon.Requests.Reset();
            var response = this.SendRequest<DistributionCommon.Responses.Reset>(request);
            if (response != default(DistributionCommon.Responses.Reset))
            {
                return response.Success;
            }

            return false;
        }

        public bool Sleep(int id)
        {
            var request = new DistributionCommon.Requests.Sleep(id);
            var response = this.SendRequest<DistributionCommon.Responses.Sleep>(request);
            if (response != default(DistributionCommon.Responses.Sleep))
            {
                return response.Success;
            }

            return false;
        }

        public bool Status()
        {
            var request = new DistributionCommon.Requests.Status();
            var response = this.SendRequest<DistributionCommon.Responses.Status>(request);
            if (response != default(DistributionCommon.Responses.Status))
            {
                return response.Constructed;
            }

            return false;
        }

        public bool Wake(int id)
        {
            var request = new DistributionCommon.Requests.Wake(id);
            var response = this.SendRequest<DistributionCommon.Responses.Wake>(request);
            if (response != default(DistributionCommon.Responses.Wake))
            {
                return response.Success;
            }

            return false;
        }

        private void OnLostNode()
        {
            if (this.LostNode != null)
            {
                this.LostNode();
            }
        }

        private void OnRecoveredNode()
        {
            if (this.RecoveredNode != null)
            {
                this.RecoveredNode();
            }
        }

        private T SendRequest<T>(DistributionCommon.Requests.Base request)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Error;

                string responseString = this.client.Send(JsonConvert.SerializeObject(request));
                if (responseString != DistributionCommon.Constants.Communication.InvalidRequestResponse)
                {
                    var baseResponse = JsonConvert.DeserializeObject<DistributionCommon.Responses.Base>(responseString);
                    if (baseResponse.ResponseType == typeof(T))
                    {
                        return JsonConvert.DeserializeObject<T>(responseString);
                    }
                }

                throw new JsonException();
            }
            catch (JsonException)
            {
                return default(T);
            }
        }

        private void PingLoop(int delay)
        {
            var client = new TcpClient();
            var nodeEP = new IPEndPoint(this.Schematic.Address, this.Schematic.Port);
            while (true)
            {
                try
                {
                    client.Connect(nodeEP);
                    client.Close();

                    if (!this.Reachable)
                    {
                        this.Reachable = true;
                        this.OnRecoveredNode();
                    }
                }
                catch (SocketException)
                {
                    if (this.Reachable)
                    {
                        this.OnLostNode();
                    }
                }

                Thread.Sleep(delay);
            }
        }

        internal sealed class InitializationException : Exception
        {
            public InitializationException() : base()
            {
            }
        }
    }
}
