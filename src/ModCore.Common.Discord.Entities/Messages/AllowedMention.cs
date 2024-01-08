using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Messages
{
    public record AllowedMention
    {
        [JsonPropertyName("parse")]
        public string[] Parse { get; set; }

        [JsonPropertyName("roles")]
        public Optional<List<Snowflake>> Roles { get; set; }

        [JsonPropertyName("users")]
        public Optional<List<Snowflake>> Users { get; set; }

        [JsonPropertyName("replied_user")]
        public Optional<bool> RepliedUser { get; set; }
    }
}