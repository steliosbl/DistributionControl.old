namespace DistributionCommon.Interface.Requests
{
    public sealed class WakeNode : Base
    {
        public WakeNode(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
