namespace DistributionCommon.Interface.Requests
{
    public sealed class AddNode : Base
    {
        public AddNode(Schematic.Node node, bool addToSchematic) : base()
        {
            this.Node = node;
            this.AddToSchematic = addToSchematic;
        }

        public Schematic.Node Node { get; private set; }
        
        public bool AddToSchematic { get; private set; }
    }
}
