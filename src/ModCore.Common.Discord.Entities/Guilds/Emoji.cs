using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record Emoji
    {
        [JsonPropertyName("id")]
        public Snowflake? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Role[]> Roles { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("require_colons")]
        public Optional<bool> RequireColons { get; set; }

        [JsonPropertyName("managed")]
        public Optional<bool> Managed { get; set; }

        [JsonPropertyName("animated")]
        public Optional<bool> Animated { get; set; }

        [JsonPropertyName("available")]
        public Optional<bool> Available { get; set; }
    }
}