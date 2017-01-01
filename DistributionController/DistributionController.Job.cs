namespace DistributionController
{
    public sealed class Job : DistributedJob.Base
    {
        public readonly bool Redundant;
        public readonly bool Balanced;

        [Newtonsoft.Json.JsonConstructor]
        public Job(DistributedJob.Blueprint blueprint, int nodeID, bool redundant, bool balanced, bool awake) : base(blueprint)
        {
            this.NodeID = nodeID;
            this.Redundant = redundant;
            this.Balanced = balanced;
            this.Awake = awake;
        }

        public int NodeID { get; private set; }

        public bool Awake { get; private set; }

        public void Transfer(int nodeID)
        {
            this.NodeID = nodeID;
        }

        public void Sleep()
        {
            this.Awake = false;
        }

        public void Wake()
        {
            this.Awake = true;
        }

        public override void Work()
        {
            throw new System.NotImplementedException();
        }
    }
}
