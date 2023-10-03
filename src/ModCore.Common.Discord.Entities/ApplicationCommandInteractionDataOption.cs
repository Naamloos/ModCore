using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record ApplicationCommandInteractionDataOption
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public ApplicationCommandType Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandInteractionDataOption> Options { get; set; }

        [JsonPropertyName("focused")]
        public Optional<bool> Focused { get; set; }
    }
}