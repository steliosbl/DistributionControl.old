namespace DistributionCommon.Requests
{
    public sealed class Wake : Base
    {
        public Wake(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
