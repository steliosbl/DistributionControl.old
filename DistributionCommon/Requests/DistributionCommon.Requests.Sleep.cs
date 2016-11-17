namespace DistributionCommon.Requests
{
    public sealed class Sleep : Base
    {
        public Sleep(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
