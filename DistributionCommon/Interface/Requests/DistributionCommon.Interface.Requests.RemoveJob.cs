namespace DistributionCommon.Interface.Requests
{
    public sealed class RemoveJob : Base
    {
        public RemoveJob(int id, bool delete) : base()
        {
            this.ID = id;
            this.Delete = delete;
        }

        public int ID { get; private set; }

        public bool Delete { get; private set; }
    }
}
