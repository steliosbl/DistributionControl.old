namespace DistributionCommon.Job
{
    public sealed class Blueprint
    {
        public Blueprint(int id)
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
