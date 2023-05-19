using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class SnowflakeJsonSerializer : JsonConverter<Snowflake>
    {
        public override Snowflake? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetUInt64();
        }

        public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
