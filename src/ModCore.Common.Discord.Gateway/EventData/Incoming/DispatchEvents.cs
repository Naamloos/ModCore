using System.Text.Json;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Gateway;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Monetization;
using ModCore.Common.Discord.Entities.Users;
using ModCore.Common.Discord.Entities.Voice;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record Resumed : IPublishable { }

    public record RateLimited : IPublishable
    {
        [JsonPropertyName("opcode")]
        public int Opcode { get; set; }

        [JsonPropertyName("retry_after")]
        public double RetryAfter { get; set; }

        [JsonPropertyName("meta")]
        public RequestGuildMemberRateLimitMetadata Meta { get; set; } = default!;
    }

    public record ApplicationCommandPermissionsUpdate
        : GuildApplicationCommandPermissions,
            IPublishable { }

    public record AutoModerationRuleCreate : AutoModerationRule, IPublishable { }

    public record AutoModerationRuleUpdate : AutoModerationRule, IPublishable { }

    public record AutoModerationRuleDelete : AutoModerationRule, IPublishable { }

    public record AutoModerationActionExecution : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("action")]
        public AutoModerationAction Action { get; set; } = default!;

        [JsonPropertyName("rule_id")]
        public Snowflake RuleId { get; set; }

        [JsonPropertyName("rule_trigger_type")]
        public int RuleTriggerType { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; set; }

        [JsonPropertyName("alert_system_message_id")]
        public Optional<Snowflake> AlertSystemMessageId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } = default!;

        [JsonPropertyName("matched_keyword")]
        public string? MatchedKeyword { get; set; }

        [JsonPropertyName("matched_content")]
        public string? MatchedContent { get; set; }
    }

    public record ChannelUpdate : Channel, IPublishable { }

    public record ChannelDelete : Channel, IPublishable { }

    public record ChannelInfo : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channels")]
        public ChannelInfoChannel[] Channels { get; set; } = default!;
    }

    public record ChannelPinsUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("last_pin_timestamp")]
        public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }
    }

    public record ThreadCreate : Channel, IPublishable
    {
        [JsonPropertyName("newly_created")]
        public Optional<bool> NewlyCreated { get; set; }
    }

    public record ThreadUpdate : Channel, IPublishable { }

    public record ThreadDelete : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("parent_id")]
        public Snowflake ParentId { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }

    public record ThreadListSync : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channel_ids")]
        public Optional<Snowflake[]> ChannelIds { get; set; }

        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; } = default!;

        [JsonPropertyName("members")]
        public ThreadMember[] Members { get; set; } = default!;
    }

    public record ThreadMemberUpdate : ThreadMember, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record ThreadMembersUpdate : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        [JsonPropertyName("added_members")]
        public Optional<ThreadMember[]> AddedMembers { get; set; }

        [JsonPropertyName("removed_member_ids")]
        public Optional<Snowflake[]> RemovedMemberIds { get; set; }
    }

    public record EntitlementCreate : Entitlement, IPublishable { }

    public record EntitlementUpdate : Entitlement, IPublishable { }

    public record EntitlementDelete : Entitlement, IPublishable { }

    public record GuildDelete : UnavailableGuild, IPublishable { }

    public record GuildBanAdd : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;
    }

    public record GuildBanRemove : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;
    }

    public record GuildEmojisUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("emojis")]
        public JsonElement[] Emojis { get; set; } = default!;
    }

    public record GuildStickersUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("stickers")]
        public JsonElement[] Stickers { get; set; } = default!;
    }

    public record GuildIntegrationsUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record GuildMemberAdd : Member, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record GuildMemberRemove : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;
    }

    public record GuildMemberUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("roles")]
        public Snowflake[] Roles { get; set; } = default!;

        [JsonPropertyName("user")]
        public User User { get; set; } = default!;

        [JsonPropertyName("nick")]
        public Optional<string?> Nick { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("joined_at")]
        public DateTimeOffset? JoinedAt { get; set; }

        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }

        [JsonPropertyName("deaf")]
        public Optional<bool> Deaf { get; set; }

        [JsonPropertyName("mute")]
        public Optional<bool> Mute { get; set; }

        [JsonPropertyName("pending")]
        public Optional<bool> Pending { get; set; }

        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }

        [JsonPropertyName("avatar_decoration_data")]
        public Optional<AvatarDecorationData?> AvatarDecorationData { get; set; }

        [JsonPropertyName("collectibles")]
        public Optional<Collectible?> Collectibles { get; set; }
    }

    public record GuildMembersChunk : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("members")]
        public Member[] Members { get; set; } = default!;

        [JsonPropertyName("chunk_index")]
        public int ChunkIndex { get; set; }

        [JsonPropertyName("chunk_count")]
        public int ChunkCount { get; set; }

        [JsonPropertyName("not_found")]
        public Optional<JsonElement[]> NotFound { get; set; }

        [JsonPropertyName("presences")]
        public Optional<GatewayPresenceEvent[]> Presences { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }
    }

    public record GuildRoleCreate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("role")]
        public Role Role { get; set; } = default!;
    }

    public record GuildRoleUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("role")]
        public Role Role { get; set; } = default!;
    }

    public record GuildRoleDelete : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("role_id")]
        public Snowflake RoleId { get; set; }
    }

    public record GuildScheduledEventCreate : GuildScheduledEvent, IPublishable { }

    public record GuildScheduledEventUpdate : GuildScheduledEvent, IPublishable { }

    public record GuildScheduledEventDelete : GuildScheduledEvent, IPublishable { }

    public record GuildScheduledEventUserAdd : IPublishable
    {
        [JsonPropertyName("guild_scheduled_event_id")]
        public Snowflake GuildScheduledEventId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record GuildScheduledEventUserRemove : IPublishable
    {
        [JsonPropertyName("guild_scheduled_event_id")]
        public Snowflake GuildScheduledEventId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record GuildSoundboardSoundCreate : SoundboardSound, IPublishable { }

    public record GuildSoundboardSoundUpdate : SoundboardSound, IPublishable { }

    public record GuildSoundboardSoundDelete : IPublishable
    {
        [JsonPropertyName("sound_id")]
        public Snowflake SoundId { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record GuildSoundboardSoundsUpdate : IPublishable
    {
        [JsonPropertyName("soundboard_sounds")]
        public SoundboardSound[] SoundboardSounds { get; set; } = default!;

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record SoundboardSounds : IPublishable
    {
        [JsonPropertyName("soundboard_sounds")]
        public SoundboardSound[] SoundboardSoundsValue { get; set; } = default!;

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record IntegrationCreate : Integration, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record IntegrationUpdate : Integration, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }
    }

    public record IntegrationDelete : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; set; }
    }

    public record InviteCreate : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("inviter")]
        public Optional<User> Inviter { get; set; }

        [JsonPropertyName("max_age")]
        public int MaxAge { get; set; }

        [JsonPropertyName("max_uses")]
        public int MaxUses { get; set; }

        [JsonPropertyName("target_type")]
        public Optional<int> TargetType { get; set; }

        [JsonPropertyName("target_user")]
        public Optional<User> TargetUser { get; set; }

        [JsonPropertyName("target_application")]
        public Optional<Application> TargetApplication { get; set; }

        [JsonPropertyName("temporary")]
        public bool Temporary { get; set; }

        [JsonPropertyName("uses")]
        public int Uses { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTimeOffset? ExpiresAt { get; set; }

        [JsonPropertyName("role_ids")]
        public Optional<Snowflake[]> RoleIds { get; set; }
    }

    public record InviteDelete : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;
    }

    public record MessageReactionAdd : IPublishable
    {
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }

        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; } = default!;

        [JsonPropertyName("message_author_id")]
        public Optional<Snowflake> MessageAuthorId { get; set; }

        [JsonPropertyName("burst")]
        public bool Burst { get; set; }

        [JsonPropertyName("burst_colors")]
        public Optional<string[]> BurstColors { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }

    public record MessageReactionRemove : IPublishable
    {
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; } = default!;

        [JsonPropertyName("burst")]
        public bool Burst { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }

    public record MessageReactionRemoveAll : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }
    }

    public record MessageReactionRemoveEmoji : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; } = default!;
    }

    public record PresenceUpdate : GatewayPresenceEvent, IPublishable { }

    public record StageInstanceCreate : StageInstance, IPublishable { }

    public record StageInstanceUpdate : StageInstance, IPublishable { }

    public record StageInstanceDelete : StageInstance, IPublishable { }

    public record SubscriptionCreate : Subscription, IPublishable { }

    public record SubscriptionUpdate : Subscription, IPublishable { }

    public record SubscriptionDelete : Subscription, IPublishable { }

    public record TypingStart : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }
    }

    public record UserUpdate : User, IPublishable { }

    public record VoiceChannelEffectSend : IPublishable
    {
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("emoji")]
        public Optional<Emoji?> Emoji { get; set; }

        [JsonPropertyName("animation_type")]
        public Optional<int?> AnimationType { get; set; }

        [JsonPropertyName("animation_id")]
        public Optional<int> AnimationId { get; set; }

        [JsonPropertyName("sound_id")]
        public Optional<Snowflake> SoundId { get; set; }

        [JsonPropertyName("sound_volume")]
        public Optional<double> SoundVolume { get; set; }
    }

    public record VoiceChannelStartTimeUpdate : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("voice_start_time")]
        public Optional<int?> VoiceStartTime { get; set; }
    }

    public record VoiceChannelStatusUpdate : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public record VoiceStateUpdate : VoiceState, IPublishable { }

    public record VoiceServerUpdate : IPublishable
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = default!;

        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("endpoint")]
        public string? Endpoint { get; set; }
    }

    public record WebhooksUpdate : IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }
    }

    public record MessagePollVoteAdd : IPublishable
    {
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("answer_id")]
        public int AnswerId { get; set; }
    }

    public record MessagePollVoteRemove : IPublishable
    {
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("answer_id")]
        public int AnswerId { get; set; }
    }
}
