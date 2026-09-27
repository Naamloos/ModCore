using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record StickerPack
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("stickers")]
        public Sticker[] Stickers { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("sku_id")]
        public Snowflake SkuId { get; set; }

        [JsonPropertyName("cover_sticker_id")]
        public Optional<Snowflake> CoverStickerId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        [JsonPropertyName("banner_asset_id")]
        public Optional<Snowflake> BannerAssetId { get; set; }
    }
}
