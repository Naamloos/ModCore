using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class OptionalJsonSerializer<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return Optional<T>.None;
            }

            T? value = JsonSerializer.Deserialize<T>(ref reader, options);
            return value != null ? new Optional<T>(value) : Optional<T>.None;
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Optional<T> value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                base.WriteAsPropertyName(writer, value, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
