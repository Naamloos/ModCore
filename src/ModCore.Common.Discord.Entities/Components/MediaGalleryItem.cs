using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Components
{
    public class MediaGalleryItem
    {
        [JsonPropertyName("media")]
        public UnfurledMediaItem Media { get; set; } = default!;

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }

        [JsonPropertyName("spoiler")]
        public Optional<bool> Spoiler { get; set; }
    }
}
