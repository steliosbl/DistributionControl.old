namespace DistributionCommon.Responses
{
    public sealed class Sleep : Base
    {
        public Sleep(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}