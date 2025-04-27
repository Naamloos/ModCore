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
    public class StringSelect : InteractiveComponent
    {
        [JsonPropertyName("type")]
        public override ComponentType Type { get; set; } = ComponentType.StringSelect;

        [JsonPropertyName("options")]
        public List<SelectOption> Options { get; set; } = new List<SelectOption>();

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; } = Optional<string>.None;

        [JsonPropertyName("min_values")]
        public Optional<int> MinValues { get; set; } = Optional<int>.None;

        [JsonPropertyName("max_values")]
        public Optional<int> MaxValues { get; set; } = Optional<int>.None;

        [JsonPropertyName("disabled")]
        public Optional<bool> Disabled { get; set; } = Optional<bool>.None;
    }

    public class SelectOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public Emoji? Emoji { get; set; } = null;
        public bool? Default { get; set; } = false;
    }
}
