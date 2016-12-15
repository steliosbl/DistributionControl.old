namespace DistributionCommon.Comm.Responses
{
    public sealed class Remove : Base
    {
        public Remove(bool success) : base()
        {
            this.Success = success;
        }

        public bool Success { get; private set; }
    }
}