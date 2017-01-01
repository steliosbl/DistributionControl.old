namespace DistributionCommon.Interface.Requests
{
    public sealed class SleepNode : Base
    {
        public SleepNode(int id, bool reassignJobs) : base()
        {
            this.ID = id;
            this.ReAssignJobs = reassignJobs;
        }

        public int ID { get; private set; }

        public bool ReAssignJobs { get; private set; }
    }
}
