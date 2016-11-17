namespace DistributionCommon.Job
{
    public abstract class Base
    {
        public Base(Blueprint blueprint)
        {
            this.Blueprint = blueprint;
        }

        public abstract void Work();

        public Blueprint Blueprint { get; private set; }
    }
}
