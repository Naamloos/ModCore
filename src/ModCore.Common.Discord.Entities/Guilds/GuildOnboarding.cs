using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildOnboarding
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("prompts")]
        public OnboardingPrompt[] Prompts { get; set; } = default!;

        [JsonPropertyName("default_channel_ids")]
        public Snowflake[] DefaultChannelIds { get; set; } = default!;

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("mode")]
        public int Mode { get; set; }
    }
}
