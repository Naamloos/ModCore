using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Webhooks
{
    public record Webhook
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake?> GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("token")]
        public Optional<string> Token { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; set; }

        [JsonPropertyName("source_guild")]
        public Optional<Guild> SourceGuild { get; set; }

        [JsonPropertyName("source_channel")]
        public Optional<Channel> SourceChannel { get; set; }

        [JsonPropertyName("url")]
        public Optional<string> Url { get; set; }
    }
}
