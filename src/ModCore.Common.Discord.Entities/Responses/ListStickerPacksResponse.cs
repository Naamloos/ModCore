using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Responses
{
    public record ListStickerPacksResponse
    {
        [JsonPropertyName("sticker_packs")]
        public StickerPack[] StickerPacks { get; set; } = default!;
    }
}
