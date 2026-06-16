using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class SnowflakeJsonSerializer : JsonConverter<Snowflake>
    {
        public override Snowflake Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => ParseSnowflake(reader.GetString()),
                JsonTokenType.Number => reader.TryGetUInt64(out var value)
                    ? value
                    : throw new JsonException("Snowflake number value was not a valid UInt64."),

                _ => throw new JsonException(
                    $"Cannot deserialize Snowflake from token {reader.TokenType}.")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            Snowflake value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override Snowflake ReadAsPropertyName(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return ParseSnowflake(reader.GetString());
        }

        public override void WriteAsPropertyName(
            Utf8JsonWriter writer,
            Snowflake value,
            JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }

        private static Snowflake ParseSnowflake(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException("Snowflake value was null or empty.");
            }

            if (!ulong.TryParse(
                    value,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var snowflake))
            {
                throw new JsonException($"Invalid Snowflake value '{value}'.");
            }

            return snowflake;
        }
    }
}