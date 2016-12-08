namespace DistributionCommon.Job
{
    public class Custom : Base
    {
        public Custom(Blueprint blueprint) : base(blueprint)
        {
        }

        public override void Work()
        {
#if DEBUG
            while (true)
            {
                try
                {
                    using (var wb = new System.Net.WebClient())
                    {
                        var response = wb.DownloadString(this.Blueprint.Address);
                    }

                    System.Threading.Thread.Sleep(this.Blueprint.Delay);
                }
                catch (System.Net.WebException)
                {
                }
            }
#else
            throw new System.NotImplementedException();
#endif
        }
    }
}
