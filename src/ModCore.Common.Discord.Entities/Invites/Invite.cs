using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Common.Discord.Entities.Invites
{
    public record Invite
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;

        [JsonPropertyName("guild")]
        public Optional<Guild> Guild { get; set; }

        [JsonPropertyName("channel")]
        public Channel? Channel { get; set; }

        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; set; }

        [JsonPropertyName("target_type")]
        public Optional<int> TargetType { get; set; }

        [JsonPropertyName("target_user")]
        public Optional<User> TargetUser { get; set; }

        [JsonPropertyName("target_application")]
        public Optional<Application> TargetApplication { get; set; }

        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }

        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTimeOffset? ExpiresAt { get; set; }

        [JsonPropertyName("guild_scheduled_event")]
        public Optional<GuildScheduledEvent> GuildScheduledEvent { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Role[]> Roles { get; set; }
    }
}
