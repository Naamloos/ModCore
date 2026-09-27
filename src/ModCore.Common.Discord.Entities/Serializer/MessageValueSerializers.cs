using System.Text.Json;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public sealed class NonceJsonSerializer : JsonConverter<Optional<string>>
    {
        public override Optional<string> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType == JsonTokenType.String)
                return new Optional<string>(reader.GetString());
            if (reader.TokenType == JsonTokenType.Null)
                return new Optional<string>(null);
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException("Nonce must be a string or number.");
            using var value = JsonDocument.ParseValue(ref reader);
            return new Optional<string>(value.RootElement.GetRawText());
        }

        public override void Write(
            Utf8JsonWriter writer,
            Optional<string> value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.Value);
    }

    public sealed class SingleAttachmentJsonSerializer : JsonConverter<Optional<Attachment>>
    {
        public override Optional<Attachment> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var attachments = JsonSerializer.Deserialize<Attachment[]>(ref reader, options);
            if (attachments?.Length > 1)
                throw new JsonException(
                    "Use the typed request's attachment array for multiple attachments."
                );
            return attachments?.Length == 1
                ? new Optional<Attachment>(attachments[0])
                : Optional<Attachment>.None;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Optional<Attachment> value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStartArray();
            if (value.HasValue && value.Value != null)
                JsonSerializer.Serialize(writer, value.Value, options);
            writer.WriteEndArray();
        }
    }
}
