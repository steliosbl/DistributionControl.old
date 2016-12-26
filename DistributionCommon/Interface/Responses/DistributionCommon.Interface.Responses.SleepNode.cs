namespace DistributionCommon.Interface.Responses
{
    public sealed class SleepNode : Base
    {
        public SleepNode(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
