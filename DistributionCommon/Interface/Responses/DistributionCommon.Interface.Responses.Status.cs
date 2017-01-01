namespace DistributionCommon.Interface.Responses
{
    using System;
    using System.Collections.Generic;

    public sealed class Status : Base
    {
        public Status (Interface.Status currentStatus) : base()
        {
            this.CurrentStatus = currentStatus;
        }

        public Interface.Status CurrentStatus { get; private set; }
    }
}
