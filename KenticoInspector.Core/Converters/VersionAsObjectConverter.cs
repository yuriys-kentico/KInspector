using System;

using Newtonsoft.Json;

namespace KenticoInspector.Core.Converters
{
    public class VersionAsObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Version);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    break;

                case Version version:
                    serializer.Serialize(writer, new { version.Major, version.Minor, version.Build, version.Revision });
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}