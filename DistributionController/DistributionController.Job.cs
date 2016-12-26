namespace DistributionController
{
    public sealed class Job : DistributedJob.Base
    {
        public readonly bool Redundant;
        public readonly bool Balanced;

        [Newtonsoft.Json.JsonConstructor]
        public Job(DistributedJob.Blueprint blueprint, int nodeID, bool redundant, bool balanced, int state) : base(blueprint)
        {
            this.NodeID = nodeID;
            this.Redundant = redundant;
            this.Balanced = balanced;
            this.State = state;
        }

        public int NodeID { get; private set; }

        public int State { get; private set; }

        public void Transfer(int nodeID)
        {
            this.NodeID = nodeID;
        }

        public void UpdateState(int state)
        {
            this.State = state;
        }

        public override void Work()
        {
            throw new System.NotImplementedException();
        }
    }
}
