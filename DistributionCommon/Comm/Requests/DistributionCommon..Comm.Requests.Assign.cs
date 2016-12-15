namespace DistributionCommon.Comm.Requests
{
    public sealed class Assign : Base
    {
        public Assign(DistributionCommon.Job.Blueprint blueprint) : base()
        {
            this.Blueprint = blueprint;
        }

        public DistributionCommon.Job.Blueprint Blueprint { get; private set; }
    }
}
