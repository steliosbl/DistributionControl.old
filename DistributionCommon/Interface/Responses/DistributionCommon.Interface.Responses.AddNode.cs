namespace DistributionCommon.Interface.Responses
{
    public sealed class AddNode : Base
    {
        public AddNode(bool success) : base()
        {
            this.Success = success;
        }
        
        public bool Success { get; private set; }
    }
}
