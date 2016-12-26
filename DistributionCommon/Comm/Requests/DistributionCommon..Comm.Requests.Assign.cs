namespace DistributionCommon.Comm.Requests
{
    public sealed class Assign : Base
    {
        public Assign(DistributedJob.Blueprint blueprint) : base()
        {
            this.Blueprint = blueprint;
        }

        public DistributedJob.Blueprint Blueprint { get; private set; }
    }
}
