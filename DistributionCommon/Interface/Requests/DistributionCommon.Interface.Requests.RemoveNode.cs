namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveNode : Base
    {
        public RemoveNode(int id, bool reassignJobs, bool sleepSaved) : base()
        {
            this.ID = id;
            this.ReAssignJobs = reassignJobs;
            this.SleepSaved = sleepSaved;
        }

        public int ID { get; private set; }

        public bool ReAssignJobs { get; private set; }

        public bool SleepSaved { get; private set; }
    }
}
