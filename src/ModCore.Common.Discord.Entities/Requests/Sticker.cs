using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateGuildStickerRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }

        [JsonPropertyName("tags")]
        public Optional<string> Tags { get; set; }
    }

    public record ModifyGuildStickerRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }

        [JsonPropertyName("tags")]
        public Optional<string> Tags { get; set; }
    }
}
