namespace DistributionCommon.Interface.Responses
{
    using System;

    public class Base
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
