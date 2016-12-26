namespace DistributionCommon.Interface.Responses
{
    public sealed class AssignNode : Base
    {
        public AssignNode(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
