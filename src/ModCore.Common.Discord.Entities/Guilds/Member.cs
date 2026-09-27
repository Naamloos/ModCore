using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Users;

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
        public Optional<Permissions> Permissions { get; set; }

        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }

        [JsonPropertyName("banner")]
        public Optional<string?> Banner { get; set; }

        [JsonPropertyName("avatar_decoration_data")]
        public Optional<AvatarDecorationData?> AvatarDecorationData { get; set; }

        [JsonPropertyName("collectibles")]
        public Optional<Collectible?> Collectibles { get; set; }
    }
}
