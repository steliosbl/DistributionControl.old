namespace DistributionCommon.Interface.Responses
{
    public sealed class SleepJob : Base
    {
        public SleepJob(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
