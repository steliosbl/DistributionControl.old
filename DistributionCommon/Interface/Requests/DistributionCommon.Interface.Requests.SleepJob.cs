namespace DistributionCommon.Interface.Requests
{
    public sealed class SleepJob : Base
    {
        public SleepJob(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
