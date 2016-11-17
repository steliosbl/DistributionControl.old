namespace DistributionCommon.Responses
{
    public sealed class Assign : Base
    {
        public Assign(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
