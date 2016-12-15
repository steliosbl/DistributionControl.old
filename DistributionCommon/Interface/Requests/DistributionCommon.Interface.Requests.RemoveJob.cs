namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveJob : Base
    {
        public RemoveJob(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
