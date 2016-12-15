namespace DistributionCommon.Comm.Requests
{
    public sealed class Remove : Base
    {
        public Remove(int id) : base()
        {
            this.ID = id;
        }

        public int ID { get; private set; }
    }
}
