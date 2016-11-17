namespace DistributionCommon
{
    using System;

    public sealed class DistributionControlException : Exception
    {
        public DistributionControlException() : base()
        {
        }

        public DistributionControlException(string message) : base(message)
        {
        }

        public DistributionControlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DistributionControlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
