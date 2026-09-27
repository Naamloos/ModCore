using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class OptionalJsonSerializer<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            T? value = JsonSerializer.Deserialize<T>(ref reader, options);
            return new Optional<T>(value);
        }

        public override void WriteAsPropertyName(
            Utf8JsonWriter writer,
            [DisallowNull] Optional<T> value,
            JsonSerializerOptions options
        )
        {
            if (value.HasValue)
            {
                base.WriteAsPropertyName(writer, value, options);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            Optional<T> value,
            JsonSerializerOptions options
        )
        {
            if (value.HasValue)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
