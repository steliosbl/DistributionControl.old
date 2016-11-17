namespace DistributionNode
{
    internal sealed class Config
    {
        public readonly int Port;
        public readonly bool Verbose;
        public readonly bool LiveErrors;
        public readonly string LogFilename;

        [Newtonsoft.Json.JsonConstructor]
        public Config(int port, bool verbose, bool liveErrors, string logFilename)
        {
            this.Port = port;
            this.Verbose = verbose;
            this.LiveErrors = liveErrors;
            this.LogFilename = logFilename;
        }
    }
}
