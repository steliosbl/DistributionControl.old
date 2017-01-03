namespace DistributionCommon.Serialization
{
    using System;
    using Newtonsoft.Json;

    public sealed class CustomSettings : JsonSerializerSettings
    {
        public CustomSettings() : base()
        {
            Converters.Add(new IPAddressConverter());
            Converters.Add(new IPEndPointConverter());
            Converters.Add(new TypeConverter());
        }
    }
}
