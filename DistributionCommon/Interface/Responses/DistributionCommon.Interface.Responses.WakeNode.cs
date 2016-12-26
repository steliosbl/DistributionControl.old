namespace DistributionCommon.Interface.Responses
{
    public sealed class WakeNode : Base
    {
        public WakeNode(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
