using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Button : InteractiveComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; } = ComponentType.Button;

        [JsonPropertyName("style")]
        public ButtonStyle Style { get; set; } = ButtonStyle.Primary;

        [JsonPropertyName("label")]
        public string? Label { get; set; } = null;

        [JsonPropertyName("emoji")]
        public Emoji? Emoji { get; set; } = null;

        [JsonPropertyName("sku_id")]
        public Snowflake? SkuId { get; set; } = null;

        [JsonPropertyName("url")]
        public string? Url { get; set; } = null;

        [JsonPropertyName("disabled")]
        public bool? Disabled { get; set; } = null;
    }

    public enum ButtonStyle
    {
        Primary = 1,
        Secondary = 2,
        Success = 3,
        Danger = 4,
        Link = 5,
        Premium = 6
    }
}
