namespace DistributionCommon.Comm.Requests
{
    using System;

    public abstract class Base
    {
        public Base()
        {
            this.RequestType = this.GetType();
        }

        [Newtonsoft.Json.JsonConstructor]
        public Base(Type requestType)
        {
            this.RequestType = requestType;
        }

        public Type RequestType { get; private set; }
    }
}
