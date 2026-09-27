using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class IdiotBoolSerializer : JsonConverter<IdiotBool>
    {
        public override IdiotBool Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.Null)
                throw new JsonException("Expected a null presence marker.");
            return new IdiotBool(true); // when not present, default is written, which will be false
        }

        public override void Write(
            Utf8JsonWriter writer,
            IdiotBool value,
            JsonSerializerOptions options
        )
        {
            writer.WriteNullValue();
        }
    }
}
