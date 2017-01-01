namespace DistributionCommon.Interface
{
    public sealed class NodeInfo
    {
        public NodeInfo(Schematic.Node schematic, bool reachable, bool awake, int assignedJobs, int awakeJobs)
        {
            this.Schematic = schematic;
            this.Reachable = reachable;
            this.Awake = awake;
            this.AssignedJobs = assignedJobs;
            this.AwakeJobs = awakeJobs;
        }

        public Schematic.Node Schematic { get; private set; }

        public bool Reachable { get; private set; }

        public bool Awake { get; private set; }

        public int AssignedJobs { get; private set; }

        public int AwakeJobs { get; private set; }
    }
}
