using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Discord.Entities.Users
{
    public record Nameplate
    {
        [JsonPropertyName("sku_id")]
        public Snowflake SkuId { get; set; }

        [JsonPropertyName("asset")]
        public string Asset { get; set; } = default!;

        [JsonPropertyName("label")]
        public string Label { get; set; } = default!;

        [JsonPropertyName("palette")]
        public string Palette { get; set; } = default!;
    }
}
