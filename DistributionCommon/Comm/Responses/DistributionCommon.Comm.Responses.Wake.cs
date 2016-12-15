namespace DistributionCommon.Comm.Responses
{
    public sealed class Wake : Base
    {
        public Wake(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}