namespace DistributionCommon.Interface.Responses
{
    public sealed class WakeJob : Base
    {
        public WakeJob(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
