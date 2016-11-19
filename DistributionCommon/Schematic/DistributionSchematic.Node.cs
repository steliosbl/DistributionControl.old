namespace DistributionCommon.Schematic
{
    public struct Node
    {
        public readonly int ID;
        public readonly int Slots;
        public readonly System.Net.IPAddress Address;
        public readonly int Port;
        public readonly bool Primary;

        [Newtonsoft.Json.JsonConstructor]
        public Node(int id, int slots, System.Net.IPAddress address, int port, bool primary)
        {
            this.ID = id;
            this.Slots = slots;
            this.Address = address;
            this.Port = port;
            this.Primary = primary;
        }
    }
}
