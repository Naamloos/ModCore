using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Users
{
    public record AvatarDecorationData
    {
        [JsonPropertyName("asset")]
        public string Asset { get; set; } = default!;

        [JsonPropertyName("sku_id")]
        public Snowflake SkuId { get; set; }
    }
}
