using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record AutoModerationAction
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("metadata")]
        public Optional<ActionMetadata> Metadata { get; set; }
    }
}
