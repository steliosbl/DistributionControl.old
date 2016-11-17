namespace DistributionCommon.Responses
{
    public sealed class Status : Base
    {
        public Status(bool constructed) : base()
        {
            this.Constructed = constructed;
        }

        public bool Constructed { get; private set; }
    }
}