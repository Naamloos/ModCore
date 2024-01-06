using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class PermissionsJsonSerializer : JsonConverter<Permissions>
    {
        public override Permissions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return string.IsNullOrEmpty(value)? Permissions.None : (Permissions)long.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Permissions value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(((long)value).ToString());
        }
    }
}
