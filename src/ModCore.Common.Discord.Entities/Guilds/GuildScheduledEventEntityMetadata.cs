using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildScheduledEventEntityMetadata
    {
        [JsonPropertyName("location")]
        public Optional<string> Location { get; set; }
    }
}
