namespace DistributionCommon.Responses
{
    public sealed class Construct : Base
    {
        public Construct(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}