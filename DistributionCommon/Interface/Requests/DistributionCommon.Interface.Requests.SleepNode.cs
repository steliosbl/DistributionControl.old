namespace DistributionCommon.Interface.Requests
{
    public sealed class SleepNode : Base
    {
        public SleepNode(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
