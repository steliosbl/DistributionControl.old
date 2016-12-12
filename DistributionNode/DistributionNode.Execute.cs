namespace DistributionNode
{
    public static class Execute
    {
        [System.STAThread]
        public static void Main(string[] args)
        {
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new DistributionCommon.Serialization.CustomSettings();
            if (args.Length == 0)
            {
                new Node();
            }
            else
            {
                new Node(args[0]);
            }
        }
    }
}
