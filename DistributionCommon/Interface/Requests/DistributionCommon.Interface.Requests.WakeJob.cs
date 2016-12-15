namespace DistributionCommon.Interface.Requests
{
    public sealed class WakeJob : Base
    {
        public WakeJob(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
