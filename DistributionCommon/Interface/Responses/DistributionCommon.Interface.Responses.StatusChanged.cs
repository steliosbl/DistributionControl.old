namespace DistributionCommon.Interface.Responses
{
    public sealed class StatusChanged : Base
    {
        public StatusChanged(bool changed) : base()
        {
            this.Changed = changed;
        }

        public bool Changed { get; private set; }
    }
}
