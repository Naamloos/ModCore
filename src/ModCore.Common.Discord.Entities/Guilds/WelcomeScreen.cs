using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record WelcomeScreen
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("welcome_channels")]
        public WelcomeScreenChannel[] WelcomeChannels { get; set; }
    }
}