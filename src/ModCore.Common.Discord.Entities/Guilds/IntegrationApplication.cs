using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record IntegrationApplication
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        [JsonPropertyName("bot")]
        public Optional<User> Bot { get; set; }
    }
}
