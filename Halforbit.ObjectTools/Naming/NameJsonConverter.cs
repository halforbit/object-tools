using Newtonsoft.Json;
using System;

namespace Halforbit.ObjectTools.Naming
{
    public class NameJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            return objectType
                .GetMethod("Parse")
                .Invoke(
                    null,
                    new object[] { reader.Value });
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
