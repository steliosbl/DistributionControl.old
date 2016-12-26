namespace DistributionCommon.Interface.Responses
{
    public sealed class RemoveJob : Base
    {
        public RemoveJob(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
