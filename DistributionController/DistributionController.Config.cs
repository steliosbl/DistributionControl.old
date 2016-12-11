namespace DistributionController
{
    internal sealed class Config
    {
        public readonly bool LiveErrors;
        public readonly bool LoadBalancing;
        public readonly bool PreLoad;
        public readonly bool Redundancy;
        public readonly bool AutoAssignFailedPreLoadJobs;
        public readonly bool Verbose;
        public readonly int PingDelay;
        public readonly int Timeout;
        public readonly string LogFilename;
        public readonly string PreLoadFilename;
        public readonly string SchematicFilename;

        [Newtonsoft.Json.JsonConstructor]
        public Config(
            bool liveErrors, 
            bool loadBalancing, 
            bool preLoad, 
            bool redundancy,
            bool autoAssignFailedPreLoadJobs,
            bool verbose, 
            int pingDelay, 
            int timeout, 
            string logFilename, 
            string preLoadFilename, 
            string schematicFilename)
        {
            this.LiveErrors = liveErrors;
            this.LoadBalancing = loadBalancing;
            this.PreLoad = preLoad;
            this.Redundancy = redundancy;
            this.AutoAssignFailedPreLoadJobs = autoAssignFailedPreLoadJobs;
            this.Verbose = verbose;
            this.PingDelay = pingDelay;
            this.Timeout = timeout;
            this.LogFilename = logFilename;
            this.PreLoadFilename = preLoadFilename;
            this.SchematicFilename = schematicFilename;
        }
    }
}
