namespace DistributionCommon.Job
{
    public class Custom : Base
    {
        public Custom(Blueprint blueprint) : base(blueprint)
        {
        }

        public override void Work()
        {
            throw new System.NotImplementedException();
        }
    }
}
