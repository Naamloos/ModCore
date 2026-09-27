using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record GatewayActivity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("url")]
        public Optional<string?> Url { get; set; }

        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("timestamps")]
        public Optional<DateTimeOffset> Timestamps { get; set; }

        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; set; }

        [JsonPropertyName("status_display_type")]
        public Optional<int?> StatusDisplayType { get; set; }

        [JsonPropertyName("details")]
        public Optional<string?> Details { get; set; }

        [JsonPropertyName("details_url")]
        public Optional<string?> DetailsUrl { get; set; }

        [JsonPropertyName("state")]
        public Optional<string?> State { get; set; }

        [JsonPropertyName("state_url")]
        public Optional<string?> StateUrl { get; set; }

        [JsonPropertyName("emoji")]
        public Optional<ActivityEmoji?> Emoji { get; set; }

        [JsonPropertyName("party")]
        public Optional<ActivityParty> Party { get; set; }

        [JsonPropertyName("assets")]
        public Optional<ActivityAssets> Assets { get; set; }

        [JsonPropertyName("secrets")]
        public Optional<ActivitySecrets> Secrets { get; set; }

        [JsonPropertyName("instance")]
        public Optional<bool> Instance { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("buttons")]
        public Optional<ActivityButtons[]> Buttons { get; set; }
    }
}
