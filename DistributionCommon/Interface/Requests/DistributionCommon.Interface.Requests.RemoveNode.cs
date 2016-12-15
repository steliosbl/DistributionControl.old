namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveNode : Base
    {
        public RemoveNode(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
