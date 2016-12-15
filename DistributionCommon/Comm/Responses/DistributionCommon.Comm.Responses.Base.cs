namespace DistributionCommon.Comm.Responses
{
    using System;

    public class Base
    {
        public Base()
        {
            this.ResponseType = this.GetType();
        }

        [Newtonsoft.Json.JsonConstructor]
        public Base(Type responseType)
        {
            this.ResponseType = responseType;
        }

        public Type ResponseType { get; private set; }
    }
}
