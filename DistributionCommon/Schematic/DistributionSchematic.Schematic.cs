namespace DistributionCommon.Schematic
{
    using System.Collections.Generic;
    using System.Linq;

    public class Schematic
    {
        public readonly Dictionary<int, Node> Nodes;

        public Schematic()
        {
            this.Nodes = new Dictionary<int, DistributionCommon.Schematic.Node>();
        }

        [Newtonsoft.Json.JsonConstructor]
        public Schematic(Dictionary<int, Node> nodes)
        {
            this.Nodes = nodes;
        }
    }
}
