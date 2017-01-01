namespace DistributionCommon
{
    public sealed class Constants
    {
        public sealed class DistributionNode
        {
            public sealed class Node
            {
                public const string ConfigFilename = "DistributionNode.cfg";
            }
        }

        public sealed class DistributionController
        {
            public sealed class Controller
            {
                public const string ConfigFilename = "DistributionController.cfg";
            }

            public sealed class API
            {
                public static string[] SubDirs =
                {
                    "AddNode",
                    "AssignJob",
                    "RemoveJob",
                    "RemoveNode",
                    "SleepJob",
                    "SleepNode",
                    "Status",
                    "StatusChanged",
                    "WakeJob",
                    "WakeNode"
                };
            }

            public sealed class Job
            {
                public sealed class State
                {
                    public const int Awake = 1;
                    public const int Asleep = 0;
                }
            }
        }

        public sealed class Communication
        {
            public const string InvalidRequestResponse = "NULL";
            public const int StreamSize = 256;
        }
    }
}
