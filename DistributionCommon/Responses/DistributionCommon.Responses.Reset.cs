namespace DistributionCommon.Responses
{
    public sealed class Reset : Base
    {
        public Reset(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}
