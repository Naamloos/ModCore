using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Gateway
{
    public record ActivityAssets
    {
        [JsonPropertyName("large_image")]
        public Optional<string> LargeImage { get; set; }

        [JsonPropertyName("large_text")]
        public Optional<string> LargeText { get; set; }

        [JsonPropertyName("large_url")]
        public Optional<string> LargeUrl { get; set; }

        [JsonPropertyName("small_image")]
        public Optional<string> SmallImage { get; set; }

        [JsonPropertyName("small_text")]
        public Optional<string> SmallText { get; set; }

        [JsonPropertyName("small_url")]
        public Optional<string> SmallUrl { get; set; }

        [JsonPropertyName("invite_cover_image")]
        public Optional<string> InviteCoverImage { get; set; }
    }
}
