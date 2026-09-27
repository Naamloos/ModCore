using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class MediaGallery : Component
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = (ComponentType)12;

        [JsonPropertyName("items")]
        public MediaGalleryItem[] Items { get; set; } = default!;
    }
}
