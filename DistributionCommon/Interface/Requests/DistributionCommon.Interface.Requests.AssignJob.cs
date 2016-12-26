namespace DistributionCommon.Interface.Requests
{
    public sealed class AssignJob : Base
    {
        public AssignJob(DistributedJob.Blueprint job) : base()
        {
            this.Job = job;
        }

        public DistributedJob.Blueprint Job { get; private set; }
    }
}
