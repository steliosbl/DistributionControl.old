namespace DistributionCommon.Schematic
{
    using System.Collections.Generic;
    using System.Linq;

    public class Schematic
    {
        public readonly Dictionary<int, Node> Nodes;

        public Dictionary<int, Node> PrimaryNodes
        {
            get
            {
                return this.Nodes.Where(node => node.Value.Primary).ToDictionary(dict => dict.Key, dict => dict.Value);
            }
        }

        public Dictionary<int, Node> SecondaryNodes
        {
            get
            {
                return this.Nodes.Where(node => !node.Value.Primary).ToDictionary(dict => dict.Key, dict => dict.Value);
            }
        }
    }
}
