namespace DistributionCommon.Interface.Responses
{
    public sealed class RemoveNode : Base
    {
        public RemoveNode(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
