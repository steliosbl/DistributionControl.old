namespace DistributionCommon.Job
{
    public class Blueprint
    {
        public Blueprint(
            int id
#if DEBUG
            ,string address,
            int delay)
#endif
        {
            this.ID = id;
#if DEBUG
            this.Address = address;
            this.Delay = delay;
        }
#endif
        public int ID { get; private set; }
#if DEBUG
        public string Address { get; private set; }

        public int Delay { get; private set; }
#endif
    }
}
