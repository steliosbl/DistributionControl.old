namespace DistributedJob
{
    public abstract class Base
    {
        public Base(Blueprint blueprint)
        {
            this.Blueprint = blueprint;
        }

        public Blueprint Blueprint { get; private set; }

        public abstract void Work();
    }
}
