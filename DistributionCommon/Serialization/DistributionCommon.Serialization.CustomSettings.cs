namespace DistributionCommon.Serialization
{
    using System;
    using Newtonsoft.Json;

    public sealed class CustomSettings : JsonSerializerSettings
    {
        public CustomSettings() : base()
        {
            base.Converters.Add(new IPAddressConverter());
            base.Converters.Add(new IPEndPointConverter());   
        }
    }
}
