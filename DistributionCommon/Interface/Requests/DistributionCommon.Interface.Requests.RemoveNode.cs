namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveNode : Base
    {
        public RemoveNode(int id, bool saveJobs, bool sleepSaved) : base()
        {
            this.ID = id;
            this.SleepSaved = sleepSaved;
        }

        public int ID { get; private set; }

        public bool SaveJobs { get; private set; }

        public bool SleepSaved { get; private set; }
    }
}
