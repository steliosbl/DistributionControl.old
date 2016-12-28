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

        public Controller(string configFilename = DistributionCommon.Constants.DistributionController.Controller.ConfigFilename)
        {
            {
                string[] dependencies = { configFilename };
                if (new DistributionCommon.DependencyManager(dependencies).FindMissing().Count != 0)
                {
                    throw new DistributionCommon.DistributionControlException("Configuration file not found.");
                }

                try
                {
                    this.config = DistributionCommon.JSONFileReader.GetObject<Config>(configFilename);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    throw new DistributionCommon.DistributionControlException("Configuration file invalid.");
                }

                if (this.config == default(Config))
                {
                    throw new DistributionCommon.DistributionControlException("Configuration file invalid.");
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
                        this.logger.Log("Node ID:" + node.Value.ID.ToString() + " initialized successfully");
                    }
                    catch (Node.InitializationException)
                    {
                        this.logger.Log("Failed to initialize node", 2);
                    }
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
                    var unsuccessful = new List<int>();
                    foreach (var job in this.jobs.Where(job => job.Value.NodeID != 0))
                    {
                        try
                        {
                            if (!this.AssignJobManual(job.Value, job.Value.NodeID))
                            {
                                this.logger.Log(String.Format("Failed to load job ID:{0}",job.Key), 1);
                                unsuccessful.Add(job.Key);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            this.logger.Log(String.Format("Failed to load job ID:{0}", job.Key), 1);
                            unsuccessful.Add(job.Key);
                        }
                    }

                    foreach (int job in unsuccessful)
                    {
                        if (this.config.AutoAssignFailedPreLoadJobs)
                        {
                            this.jobs[job].Transfer(0);
                        }
                        else
                        {
                            this.jobs.Remove(job);
                        }
                    }

                    this.logger.Log("Beginning automatic assignment.");
                    var unassigned = this.jobs.Values.Where(job => job.NodeID == 0).Select(job => job.Blueprint.ID).Where(job => this.jobs[job].Balanced).ToList();
                    var remaining = this.AssignJobsBalanced(unassigned);
                    var assigned = unassigned.Where(job => !remaining.Any(rem => rem == job));
                    foreach (int job in assigned)
                    {
                        this.logger.Log("Loaded job ID:" + job);
                    }

                    foreach (int job in remaining)
                    {
                        this.logger.Log("Failed to load job ID:" + job, 1);
                    }

                    this.logger.Log("Job pre-load completed.");
                }
                else
                {
                    this.jobs = new Dictionary<int, Job>();
                }
            }

            {
                if (this.config.LoadBalancing)
                {
                    this.DistributionModification += this.BalanceAllJobs;
                }
            }

            this.logger.Log("Startup completed.");
        }

        private delegate void DistributionModificationHandler();

        private event DistributionModificationHandler DistributionModification;

        private int TotalSlots
        {
            get
            {
                return this.nodes.Values.Where(node => node.Reachable).Sum(node => node.Schematic.Slots);
            }
        }

        private int TotalSlotsAvailable
        {
            get
            {
                return this.TotalSlots - this.jobs.Count(job => job.Value.NodeID != 0);
            }
        }

        private void OnDistributionModification()
        {
            if (this.DistributionModification != null)
            {
                this.DistributionModification();
            }
        }
            
        private void LostNodeHandler(Node sender, EventArgs e)
        {
            this.logger.Log("Lost node ID:" + sender.Schematic.ID.ToString(), 1);
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
            else
            {
                this.OnDistributionModification();
            }
        }

        private List<Job> AssignedJobGetter(Node node)
        {
            return this.jobs.Values.Where(job => job.NodeID == node.Schematic.ID).ToList();
        }

        private void TimeoutHandler(Node sender, EventArgs e)
        {
            this.logger.Log("Node ID:" + sender.Schematic.ID.ToString() + " timed out.", 2);
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
            if (jobIDs.Count <= this.TotalSlotsAvailable)
            {
                var nodes = this.nodes.Where(node => node.Value.Reachable && node.Value.Schematic.AllowsAutoAssign).ToDictionary(node => node.Key, node => (float)node.Value.AssignedJobs.Count / node.Value.Schematic.Slots);

                while (jobIDs.Count > 0)
                {
                    var min = nodes.Aggregate((l, r) => l.Value < r.Value ? l : r);
                    if (min.Value != 1)
                    {
                        if (this.AssignJobManual(jobIDs[0], min.Key))
                        {
                            nodes[min.Key] = (float)this.nodes[min.Key].AssignedJobs.Count / this.nodes[min.Key].Schematic.Slots;
                            jobIDs.RemoveAt(0);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return jobIDs;
        }

        private bool TransferRedundantJobs(Node lostNode)
        {
            this.logger.Log("Beginning transfer of redundant jobs from node ID:" + lostNode.Schematic.ID.ToString());

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
            var nodes = this.nodes.Where(node => node.Value.Reachable && node.Value.Schematic.Balanced).Select(node => node.Key);

            var jobs = new List<int>();

            foreach (var node in nodes)
            {
                int idealJobs = (int)Math.Ceiling((decimal)(this.nodes[node].Schematic.Slots * this.jobs.Count) / this.TotalSlots);

                if (this.nodes[node].AssignedJobs.Count > idealJobs)
                {
                    var removeJobs = this.nodes[node].AssignedJobs.GetRange(idealJobs, this.nodes[node].AssignedJobs.Count - idealJobs).Select(job => job.Blueprint.ID);
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

        private bool AssignJobManual(int jobID, int nodeID)
        {
            return this.AssignJobManual(this.jobs[jobID], nodeID);
        }

        private bool AssignJobManual(Job job, int nodeID)
        {
            if (this.nodes[nodeID].Assign(job))
            {
                this.logger.Log(String.Format("Assigned job ID:{0} to node ID:{1}", job.Blueprint.ID, nodeID));
                job.Transfer(nodeID);
                if (job.State == 1)
                {
                    this.logger.Log(String.Format("Awoke job ID:{0}", job.Blueprint.ID));
                    this.nodes[nodeID].Wake(job.Blueprint.ID);
                }

                return true;
            }

            return false;
        }
    }
}
