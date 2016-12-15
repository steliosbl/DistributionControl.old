namespace DistributionCommon.Comm.Requests
{
    public sealed class Construct : Base
    {
        public Construct(DistributionCommon.Schematic.Node schematic) : base()
        {
            this.Schematic = schematic;
        }

        public DistributionCommon.Schematic.Node Schematic { get; private set; }
    }
}
