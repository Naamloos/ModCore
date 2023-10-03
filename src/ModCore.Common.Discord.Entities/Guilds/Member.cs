using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record Member
    {
        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("nick")]
        public Optional<string?> Nickname { get; set; }

        [JsonPropertyName("avatar")]
        public Optional<string?> Avatar { get; set; }

        [JsonPropertyName("roles")]
        public Snowflake[] Roles { get; set; }

        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }

        [JsonPropertyName("deaf")]
        public bool Deafened { get; set; }

        [JsonPropertyName("mute")]
        public bool Muted { get; set; }

        [JsonPropertyName("flags")]
        public GuildMemberFlags Flags { get; set; }

        [JsonPropertyName("pending")]
        public Optional<bool> Pending { get; set; }

        [JsonPropertyName("permissions")]
        public Optional<string> Permissions { get; set; }

        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }
    }
}