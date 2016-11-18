namespace DistributionCommon
{
    public sealed class Constants
    {
        public sealed class DistributionNode
        {
            public sealed class NetListener
            {
                public const int StreamSize = 256;
            }

            public sealed class Node
            {
                public const string ConfigFilename = "DistributionNode.cfg";
                public const string InvalidRequestResponse = "NULL";
            }
        }

        public sealed class DistributionController
        {
            public sealed class NetClient
            {
                public const int StreamSize = 256;
            }
        }
    }
}
