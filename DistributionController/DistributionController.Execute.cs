namespace DistributionController
{
    public static class Execute
    {
        [System.STAThread]
        public static void Main(string[] args)
        {
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new DistributionCommon.Serialization.CustomSettings();
            new Controller();
        }
    }
}
