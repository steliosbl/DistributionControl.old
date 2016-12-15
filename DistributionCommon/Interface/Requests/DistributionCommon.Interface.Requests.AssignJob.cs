namespace DistributionCommon.Interface.Requests
{
    public sealed class AssignJob : Base
    {
        public AssignJob(Job.Blueprint job) : base()
        {
            this.Job = job;
        }

        public Job.Blueprint Job { get; private set; }
    }
}
