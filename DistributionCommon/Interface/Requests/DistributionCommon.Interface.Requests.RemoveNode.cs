namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveNode : Base
    {
        public RemoveNode(int id, bool reassignJobs) : base()
        {
            this.ID = id;
            this.ReAssignJobs = reassignJobs;
        }

        public int ID { get; private set; }

        public bool ReAssignJobs { get; private set; }
    }
}
