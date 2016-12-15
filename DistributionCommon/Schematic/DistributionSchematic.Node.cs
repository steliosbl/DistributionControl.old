namespace DistributionCommon.Schematic
{
    public struct Node
    {
        public readonly int ID;
        public readonly int Slots;
        public readonly System.Net.IPAddress Address;
        public readonly int Port;
        public readonly bool Balanced;
        public readonly bool AllowsAutoAssign;

        [Newtonsoft.Json.JsonConstructor]
        public Node(int id, int slots, System.Net.IPAddress address, int port, bool balanced, bool allowsAutoAssign)
        {
            this.ID = id;
            this.Slots = slots;
            this.Address = address;
            this.Port = port;
            this.Balanced = balanced;
            this.AllowsAutoAssign = allowsAutoAssign;
        }
    }
}
