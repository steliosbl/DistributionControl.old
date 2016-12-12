namespace DistributionController
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Newtonsoft.Json;

    internal sealed class Node
    {
        public readonly DistributionCommon.Schematic.Node Schematic;
        private NetClient client;
        private Thread watchdog;
        private System.Timers.Timer timeoutTimer;
        private AssignedJobGetter assignedJobs;

        public Node(DistributionCommon.Schematic.Node schematic, LostNodeHandler lostHandler, RecoveredNodeHandler recoveredHandler, TimeoutHandler timeoutHandler, AssignedJobGetter jobGetter, int pingDelay)
        {
            this.Schematic = schematic;
            this.Reachable = false;
            this.client = new NetClient(this.Schematic.Address, this.Schematic.Port, this.RequestFailedHandler);

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
                this.watchdog.Abort();
                throw new InitializationException();
            }

            this.LostNode += lostHandler;
            this.RecoveredNode = recoveredHandler;
            this.Timeout += timeoutHandler;
            this.assignedJobs = jobGetter;
        }

        public delegate void LostNodeHandler(Node sender, EventArgs e);

        public delegate void RecoveredNodeHandler(Node sender, EventArgs e);

        public delegate void TimeoutHandler(Node sender, EventArgs e);

        public delegate List<Job> AssignedJobGetter(Node sender);

        public event LostNodeHandler LostNode;
        
        public event RecoveredNodeHandler RecoveredNode;

        public event TimeoutHandler Timeout;

        public bool Reachable { get; private set; }
        
        public List<Job> AssignedJobs
        {
            get
            {
                return this.assignedJobs(this);
            }
        }

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

        public void BeginCountdown(int duration)
        {
            this.timeoutTimer = new System.Timers.Timer(Convert.ToDouble(duration));
            this.timeoutTimer.AutoReset = false;
            this.timeoutTimer.Elapsed += (s, e) => { this.OnTimeout(EventArgs.Empty); };
            this.timeoutTimer.Start();
        }

        public void InterruptCountdown()
        {
            if (this.timeoutTimer.Enabled)
            {
                this.timeoutTimer.Stop();
            }

            this.timeoutTimer = null;
        }

        private void OnLostNode(EventArgs e)
        {
            if (this.LostNode != null)
            {
                this.LostNode(this, e);
            }
        }

        private void OnRecoveredNode(EventArgs e)
        {
            if (this.RecoveredNode != null)
            {
                this.RecoveredNode(this, e);
            }
        }

        private void OnTimeout(EventArgs e)
        {
            if (this.Timeout != null)
            {
                this.Timeout(this, e);
            }
        }

        private void RequestFailedHandler(EventArgs e)
        {
            if (this.Reachable)
            {
                this.Reachable = false;
                this.OnLostNode(e);
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
            while (true)
            {
                bool success = false;
                try
                {
                    var client = new TcpClient();
                    success = client.BeginConnect(this.Schematic.Address, this.Schematic.Port, null, null).AsyncWaitHandle.WaitOne(delay);
                    client.Close();
                }
                catch (SocketException)
                {
                }
                finally
                {
                    if (success)
                    {
                        if (!this.Reachable)
                        {
                            this.Reachable = true;
                            this.OnRecoveredNode(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (this.Reachable)
                        {
                            this.OnLostNode(EventArgs.Empty);
                            this.Reachable = false;
                        }
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
