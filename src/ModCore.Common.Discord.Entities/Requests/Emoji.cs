using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateGuildEmojiRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("image")]
        public Optional<string> Image { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Snowflake[]> Roles { get; set; }
    }

    public record ModifyGuildEmojiRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Snowflake[]?> Roles { get; set; }
    }

    public record CreateApplicationEmojiRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("image")]
        public Optional<string> Image { get; set; }
    }

    public record ModifyApplicationEmojiRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
    }
}
