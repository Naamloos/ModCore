using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildWidget
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("instant_invite")]
        public string? InstantInvite { get; set; }

        [JsonPropertyName("channels")]
        public Channel[] Channels { get; set; } = default!;

        [JsonPropertyName("members")]
        public User[] Members { get; set; } = default!;

        [JsonPropertyName("presence_count")]
        public int PresenceCount { get; set; }
    }
}
