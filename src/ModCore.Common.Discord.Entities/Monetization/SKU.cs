using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Monetization
{
    public record SKU
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = default!;

        [JsonPropertyName("flags")]
        public long Flags { get; set; }
    }
}
