namespace DistributionController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class Controller
    {
        private Config config;
        private Dictionary<int, Job> jobs;
        private Dictionary<int, Node> nodes;
        private DistributionCommon.Logger logger;
        private DistributionCommon.Schematic.Schematic schematic;

        public Controller()
        {
            {
                string[] dependencies = { DistributionCommon.Constants.DistributionController.Controller.ConfigFilename };
                if (new DistributionCommon.DependencyManager(dependencies).FindMissing().Count != 0)
                {
                    throw new DistributionCommon.DistributionControlException("Configuration file not found.");
                }

                this.config = DistributionCommon.JSONFileReader.GetObject<Config>(DistributionCommon.Constants.DistributionController.Controller.ConfigFilename);

                if (this.config == default(Config))
                {
                    throw new DistributionCommon.DistributionControlException("Unable to read configuration file.");
                }
            }

            { 
            this.logger = new DistributionCommon.Logger(this.config.LogFilename, this.config.Verbose);
            this.logger.Log("Starting up controller...");
            }

            bool preloadPossible = true;
            {
                var dependencies = new List<string>();
                dependencies.Add(this.config.SchematicFilename);
                if (this.config.PreLoad)
                {
                    dependencies.Add(this.config.PreLoadFilename);
                }

                var missingFiles = new DistributionCommon.DependencyManager(dependencies.ToArray()).FindMissing();

                if (missingFiles.Contains(this.config.SchematicFilename))
                {
                    this.logger.Log("Schematic file not found.", 3);
                    throw new DistributionCommon.DistributionControlException("Schematic file not found.");
                }

                if (missingFiles.Contains(this.config.PreLoadFilename))
                {
                    this.logger.Log("Pre-load file not found.", 1);
                    preloadPossible = false;
                }
            }

            {
                this.schematic = DistributionCommon.JSONFileReader.GetObject<DistributionCommon.Schematic.Schematic>(this.config.SchematicFilename);
                this.nodes = new Dictionary<int, Node>();
                foreach (var node in this.schematic.Nodes)
                {
                    try
                    {
                        this.logger.Log("Initializing node ID:" + node.Value.ID);
                        this.nodes.Add(node.Value.ID, new Node(node.Value, this.LostNodeHandler, this.RecoveredNodeHandler, this.TimeoutHandler, this.AssignedJobGetter, this.config.PingDelay));
                    }
                    catch (Node.InitializationException)
                    {
                        this.logger.Log("Failed to initialize node", 2);
                    }
                    this.logger.Log("Node ID:" + node.Value.ID.ToString() + " initialized successfully");
                }

                if (this.nodes.Count == 0)
                {
                    this.logger.Log("All nodes failed to initialize.", 3);
                    throw new DistributionCommon.DistributionControlException("All nodes failed to initialize.");
                }
            }

            {
                if (this.config.PreLoad && preloadPossible)
                {
                    this.logger.Log("Beginning job pre-load.");
                    this.jobs = DistributionCommon.JSONFileReader.GetObject<Dictionary<int, Job>>(this.config.PreLoadFilename);
                    foreach (var job in this.jobs.Where(job => job.Value.NodeID != 0))
                    {
                        this.logger.Log("Loading job ID:" + job.Key);
                        try
                        {
                            if (!this.nodes[job.Value.NodeID].Assign(job.Value))
                            {
                                this.logger.Log("Failed to load job.", 1);
                                this.jobs.Remove(job.Key);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            this.logger.Log("Failed to load job.", 1);
                            this.jobs.Remove(job.Key);
                        }
                    }

                    this.logger.Log("Beginning automatic assignment.");
                    var a = this.jobs.Values.Where(job => job.NodeID == 0).Select(job => job.Blueprint.ID).ToList();
                    this.AssignJobsBalanced(a);
                    this.logger.Log("Job pre-load completed.");
                }
                else
                {
                    this.jobs = new Dictionary<int, Job>();
                }
            }
            this.logger.Log("Startup completed.");
        }
            
        private void LostNodeHandler(Node sender, EventArgs e)
        {
            sender.BeginCountdown(this.config.Timeout);
        }

        private void RecoveredNodeHandler(Node sender, EventArgs e)
        {
            sender.InterruptCountdown();
            this.logger.Log("Recovered node ID:" + sender.Schematic.ID.ToString());
            sender.Reset();
            if (!sender.Construct())
            {
                throw new Node.InitializationException();
            }
        }

        private List<Job> AssignedJobGetter(Node node)
        {
            return this.jobs.Values.Where(job => job.NodeID == node.Schematic.ID).ToList();
        }

        private void TimeoutHandler(Node sender, EventArgs e)
        {
            this.logger.Log("Lost node ID:" + sender.Schematic.ID.ToString(), 2);
            if (this.config.Redundancy)
            {
                if (this.TransferRedundantJobs(sender))
                {
                    this.logger.Log("Completed redundant job transfer from node ID:" + sender.Schematic.ID.ToString());
                }
                else
                {
                    this.logger.Log("Failed to transfer all redundant jobs from node ID:" + sender.Schematic.ID.ToString(), 2);
                }
            }
        }
        
        private bool AssignJobBalanced(int jobID)
        {
            var l = new List<int>();
            l.Add(jobID);
            return this.AssignJobsBalanced(l).Count == 0;
        }

        private List<int> AssignJobsBalanced(List<int> jobIDs)
        {
            var nodes = this.nodes.Where(node => node.Value.Reachable).ToDictionary(node => node.Key, node => node.Value.AssignedJobs.Count / node.Value.Schematic.Slots);
            
            while (jobIDs.Count > 0)
            {
                int min = nodes.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                if (min != 0)
                {
                    if (this.nodes[min].Assign(this.jobs[jobIDs[0]]))
                    {
                        this.logger.Log("Assigned job ID:" + jobIDs[0].ToString() + " to node ID:" + min.ToString());
                        this.jobs[jobIDs[0]].Transfer(min);
                        nodes[min] = this.nodes[min].AssignedJobs.Count / this.nodes[min].Schematic.Slots;
                        if (this.jobs[jobIDs[0]].State == 1)
                        {
                            this.logger.Log("Awoke job ID:" + jobIDs[0].ToString());
                            this.nodes[min].Wake(jobIDs[0]);
                        }
                        jobIDs.RemoveAt(0);
                    }
                }
                else
                {
                    break;
                }
            }

            return jobIDs;
        }

        private bool TransferRedundantJobs(Node lostNode)
        {
            this.logger.Log("Beginning transfer or redundant jobs from node ID:" + lostNode.Schematic.ID.ToString());

            var redundantJobs = this.jobs.Where(job => job.Value.NodeID == lostNode.Schematic.ID && job.Value.Redundant).Select(job => job.Key).ToList();

            var remainingJobs = this.AssignJobsBalanced(redundantJobs);

            foreach (int id in remainingJobs)
            {
                this.logger.Log("Failed to transfer job ID:" + id.ToString(), 1);
            }

            return remainingJobs.Count == 0;
        }

        private void BalanceAllJobs()
        {
            this.logger.Log("Beginning load balance");
            var nodes = this.nodes.Where(node => node.Value.Reachable).Select(node => node.Key);

            int totalSlots = this.nodes.Sum(node => node.Value.Schematic.Slots);

            var jobs = new List<int>();

            foreach (var node in nodes)
            {
                int idealJobs = (int)Math.Ceiling((decimal)(this.nodes[node].Schematic.Slots * this.jobs.Count) / totalSlots);

                if (this.nodes[node].AssignedJobs.Count > idealJobs)
                {
                    var removeJobs = this.nodes[node].AssignedJobs.GetRange(idealJobs, this.nodes[node].AssignedJobs.Count - 1).Select(job => job.Blueprint.ID);
                    foreach (int jobID in removeJobs)
                    {
                        this.nodes[node].Remove(jobID);
                        this.jobs[jobID].Transfer(0);
                        jobs.Add(jobID);
                    }
                }

                this.AssignJobsBalanced(jobs);
            }
        }
    }
}
