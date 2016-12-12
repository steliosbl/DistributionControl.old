namespace DistributionController
{
    public static class Execute
    {
        [System.STAThread]
        public static void Main(string[] args)
        {
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new DistributionCommon.Serialization.CustomSettings();
            if (args.Length == 0)
            {
                new Controller();
            }
            else
            {
                new Controller(args[0]);
            }
        }
    }
}
