namespace DistributionCommon.Serialization
{
    using System;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class TypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == objectType.GetType() || objectType == typeof(Type);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = (Type)value;
            writer.WriteValue(type.FullName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            return Type.GetType(token.Value<string>());
        }
    }
}
