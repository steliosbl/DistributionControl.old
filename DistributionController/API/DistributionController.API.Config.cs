namespace DistributionController.API
{
    internal sealed class Config
    {
        public readonly int Port;

        [Newtonsoft.Json.JsonConstructor]
        public Config(int port)
        {
            this.Port = port;
        }
    }
}
