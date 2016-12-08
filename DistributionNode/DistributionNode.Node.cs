namespace DistributionNode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal sealed class Node
    {
        private readonly Config config;
        private NetListener listener;
        private DistributionCommon.Schematic.Node schematic;
        private Dictionary<int, DistributedWorker.Worker> workers;
        private DistributionCommon.Logger logger;
        private bool constructed;

        public Node()
        {
            string[] dependencies = { DistributionCommon.Constants.DistributionNode.Node.ConfigFilename };
            if (new DistributionCommon.DependencyManager(dependencies).FindMissing().Count != 0)
            {
                throw new DistributionCommon.DistributionControlException("Configuration file not found.");
            }

            this.config = DistributionCommon.JSONFileReader.GetObject<Config>(DistributionCommon.Constants.DistributionNode.Node.ConfigFilename);

            if (this.config != default(Config))
            {
                this.logger = new DistributionCommon.Logger(this.config.LogFilename, this.config.Verbose);
                this.logger.Log("Starting up node...");
                this.constructed = false;
                this.workers = new Dictionary<int, DistributedWorker.Worker>();
                try
                {
                    this.logger.Log("Initializing listener...");
                    this.listener = new NetListener(this.config.Port, this.RequestSifter, this.logger.Log);
                    this.logger.Log("Startup complete");
                    var task = Task.Run(async () => { await this.listener.StartListener(); });
                    task.Wait();
                }
                catch (Exception e)
                {
                    if (!this.config.LiveErrors)
                    {
                        this.logger.Log(e.StackTrace, 3);
                        Environment.Exit(1);
                    }

                    throw;
                }
            }
            else
            {
                throw new DistributionCommon.DistributionControlException("Unable to load configuration file.");
            }
        }

        private string RequestSifter(string data)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Error;

                var baseRequest = JsonConvert.DeserializeObject<DistributionCommon.Requests.Base>(data);
                dynamic request = JsonConvert.DeserializeObject(data, baseRequest.RequestType);
                this.logger.Log("Received request {" + baseRequest.RequestType.Name + "}");
                Tuple<bool?, DistributionCommon.Responses.Base> result = this.HandleRequest(request);
                switch (result.Item1)
                {
                    case true:
                        this.logger.Log("Operation successful");
                        break;
                    case false:
                        this.logger.Log("Operation failed", 1);
                        break;
                }
                return JsonConvert.SerializeObject(result.Item2);
            }
            catch (JsonException)
            {
                this.logger.Log("Received invalid request", 1);
                return DistributionCommon.Constants.Communication.InvalidRequestResponse;
            }
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Assign request)
        {
            bool success = false;
            if (this.constructed)
            {
                if (this.schematic.Slots > this.workers.Count)
                {
                    var newWorker = new DistributedWorker.Worker(request.Blueprint);
                    this.workers.Add(request.Blueprint.ID, newWorker);
                    success = true;
                }
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Assign(success));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Construct request)
        {
            bool success = false;
            if (!this.constructed)
            {
                this.schematic = request.Schematic;
                this.constructed = true;
                success = true;
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Construct(success));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Remove request)
        {
            bool success = false;
            if (this.workers.ContainsKey(request.ID))
            {
                this.workers[request.ID].Sleep();
                this.workers.Remove(request.ID);
                success = true;
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Remove(success));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Reset request)
        {
            bool success = false;
            if (this.constructed)
            {
                this.schematic = default(DistributionCommon.Schematic.Node);
                this.constructed = false;
                foreach (var worker in this.workers.Values.ToList().FindAll(w => w.Awake))
                {
                    worker.Sleep();
                }

                this.workers = new Dictionary<int, DistributedWorker.Worker>();
                success = true;
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Reset(success));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Sleep request)
        {
            bool success = false;
            if (this.workers.ContainsKey(request.ID))
            {
                if (this.workers[request.ID].Awake)
                {
                    this.workers[request.ID].Sleep();
                    success = true;
                }
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Sleep(success));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Status request)
        {
            return new Tuple<bool?, DistributionCommon.Responses.Base>(null, new DistributionCommon.Responses.Status(this.constructed));
        }

        private Tuple<bool?, DistributionCommon.Responses.Base> HandleRequest(DistributionCommon.Requests.Wake request)
        {
            bool success = false;
            if (this.workers.ContainsKey(request.ID))
            {
                if (!this.workers[request.ID].Awake)
                {
                    this.workers[request.ID].Wake();
                    success = true;
                }
            }

            return new Tuple<bool?, DistributionCommon.Responses.Base>(success, new DistributionCommon.Responses.Wake(success));
        }
    }
}
