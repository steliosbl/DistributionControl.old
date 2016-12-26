namespace DistributionCommon.Interface.Responses
{
    public sealed class AssignJob : Base
    {
        public AssignJob(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
