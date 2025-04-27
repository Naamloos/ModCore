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
        public override ComponentType Type { get; set; } = ComponentType.Button;

        [JsonPropertyName("style")]
        public ButtonStyle Style { get; set; } = ButtonStyle.Primary;

        [JsonPropertyName("label")]
        public Optional<string> Label { get; set; } = Optional<string>.None;

        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; set; } = Optional<Emoji>.None;

        [JsonPropertyName("sku_id")]
        public Optional<Snowflake> SkuId { get; set; } = Optional<Snowflake>.None;

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; } = Optional<string>.None;

        [JsonPropertyName("disabled")]
        public Optional<bool> Disabled { get; set; } = Optional<bool>.None;
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
