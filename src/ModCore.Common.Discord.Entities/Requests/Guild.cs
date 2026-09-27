using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetGuildQuery
    {
        [JsonPropertyName("with_counts")]
        public Optional<bool> WithCounts { get; set; }
    }

    public record ModifyGuildRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("region")]
        public Optional<string?> Region { get; set; }

        [JsonPropertyName("verification_level")]
        public Optional<int?> VerificationLevel { get; set; }

        [JsonPropertyName("default_message_notifications")]
        public Optional<int?> DefaultMessageNotifications { get; set; }

        [JsonPropertyName("explicit_content_filter")]
        public Optional<int?> ExplicitContentFilter { get; set; }

        [JsonPropertyName("afk_channel_id")]
        public Optional<Snowflake?> AfkChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        public Optional<int> AfkTimeout { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; set; }

        [JsonPropertyName("splash")]
        public Optional<string?> Splash { get; set; }

        [JsonPropertyName("discovery_splash")]
        public Optional<string?> DiscoverySplash { get; set; }

        [JsonPropertyName("banner")]
        public Optional<string?> Banner { get; set; }

        [JsonPropertyName("system_channel_id")]
        public Optional<Snowflake?> SystemChannelId { get; set; }

        [JsonPropertyName("system_channel_flags")]
        public Optional<int> SystemChannelFlags { get; set; }

        [JsonPropertyName("rules_channel_id")]
        public Optional<Snowflake?> RulesChannelId { get; set; }

        [JsonPropertyName("public_updates_channel_id")]
        public Optional<Snowflake?> PublicUpdatesChannelId { get; set; }

        [JsonPropertyName("preferred_locale")]
        public Optional<string?> PreferredLocale { get; set; }

        [JsonPropertyName("features")]
        public Optional<string[]> Features { get; set; }

        [JsonPropertyName("description")]
        public Optional<string?> Description { get; set; }

        [JsonPropertyName("premium_progress_bar_enabled")]
        public Optional<bool> PremiumProgressBarEnabled { get; set; }

        [JsonPropertyName("safety_alerts_channel_id")]
        public Optional<Snowflake?> SafetyAlertsChannelId { get; set; }
    }

    public record CreateGuildChannelRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("topic")]
        public Optional<string> Topic { get; set; }

        [JsonPropertyName("bitrate")]
        public Optional<int> Bitrate { get; set; }

        [JsonPropertyName("user_limit")]
        public Optional<int> UserLimit { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> RateLimitPerUser { get; set; }

        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }

        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]> PermissionOverwrites { get; set; }

        [JsonPropertyName("parent_id")]
        public Optional<Snowflake> ParentId { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool> Nsfw { get; set; }

        [JsonPropertyName("rtc_region")]
        public Optional<string> RtcRegion { get; set; }

        [JsonPropertyName("video_quality_mode")]
        public Optional<int> VideoQualityMode { get; set; }

        [JsonPropertyName("default_auto_archive_duration")]
        public Optional<int> DefaultAutoArchiveDuration { get; set; }

        [JsonPropertyName("default_reaction_emoji")]
        public Optional<DefaultReaction> DefaultReactionEmoji { get; set; }

        [JsonPropertyName("available_tags")]
        public Optional<Tag[]> AvailableTags { get; set; }

        [JsonPropertyName("default_sort_order")]
        public Optional<int> DefaultSortOrder { get; set; }

        [JsonPropertyName("default_forum_layout")]
        public Optional<int> DefaultForumLayout { get; set; }

        [JsonPropertyName("default_thread_rate_limit_per_user")]
        public Optional<int> DefaultThreadRateLimitPerUser { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }
    }

    public record ModifyGuildChannelPositionsRequest
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("position")]
        public Optional<int?> Position { get; set; }

        [JsonPropertyName("lock_permissions")]
        public Optional<bool?> LockPermissions { get; set; }

        [JsonPropertyName("parent_id")]
        public Optional<Snowflake?> ParentId { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long?> Flags { get; set; }
    }

    public record ListGuildMembersQuery
    {
        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }
    }

    public record SearchGuildMembersQuery
    {
        [JsonPropertyName("query")]
        public Optional<string> Query { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record AddGuildMemberRequest
    {
        [JsonPropertyName("access_token")]
        public Optional<string> AccessToken { get; set; }

        [JsonPropertyName("nick")]
        public Optional<string> Nick { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Snowflake[]> Roles { get; set; }

        [JsonPropertyName("mute")]
        public Optional<bool> Mute { get; set; }

        [JsonPropertyName("deaf")]
        public Optional<bool> Deaf { get; set; }
    }

    public record ModifyGuildMemberRequest
    {
        [JsonPropertyName("nick")]
        public Optional<string> Nick { get; set; }

        [JsonPropertyName("roles")]
        public Optional<Snowflake[]> Roles { get; set; }

        [JsonPropertyName("mute")]
        public Optional<bool> Mute { get; set; }

        [JsonPropertyName("deaf")]
        public Optional<bool> Deaf { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset> CommunicationDisabledUntil { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }
    }

    public record ModifyCurrentMemberRequest
    {
        [JsonPropertyName("nick")]
        public Optional<string?> Nick { get; set; }

        [JsonPropertyName("banner")]
        public Optional<string?> Banner { get; set; }

        [JsonPropertyName("avatar")]
        public Optional<string?> Avatar { get; set; }

        [JsonPropertyName("bio")]
        public Optional<string?> Bio { get; set; }
    }

    public record ModifyCurrentUserNickRequest
    {
        [JsonPropertyName("nick")]
        public Optional<string?> Nick { get; set; }
    }

    public record GetGuildBansQuery
    {
        [JsonPropertyName("limit")]
        public Optional<double> Limit { get; set; }

        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }
    }

    public record CreateGuildBanRequest
    {
        [JsonPropertyName("delete_message_days")]
        public Optional<int> DeleteMessageDays { get; set; }

        [JsonPropertyName("delete_message_seconds")]
        public Optional<int> DeleteMessageSeconds { get; set; }
    }

    public record BulkGuildBanRequest
    {
        [JsonPropertyName("user_ids")]
        public Optional<Snowflake[]> UserIds { get; set; }

        [JsonPropertyName("delete_message_seconds")]
        public Optional<int> DeleteMessageSeconds { get; set; }
    }

    public record CreateGuildRoleRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("permissions")]
        public Optional<string> Permissions { get; set; }

        [JsonPropertyName("color")]
        public Optional<int> Color { get; set; }

        [JsonPropertyName("colors")]
        public Optional<RoleColors> Colors { get; set; }

        [JsonPropertyName("hoist")]
        public Optional<bool> Hoist { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string?> Icon { get; set; }

        [JsonPropertyName("unicode_emoji")]
        public Optional<string?> UnicodeEmoji { get; set; }

        [JsonPropertyName("mentionable")]
        public Optional<bool> Mentionable { get; set; }
    }

    public record ModifyGuildRolePositionsRequest
    {
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; set; }

        [JsonPropertyName("position")]
        public Optional<int?> Position { get; set; }
    }

    public record ModifyGuildRoleRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("permissions")]
        public Optional<string> Permissions { get; set; }

        [JsonPropertyName("color")]
        public Optional<int> Color { get; set; }

        [JsonPropertyName("colors")]
        public Optional<RoleColors> Colors { get; set; }

        [JsonPropertyName("hoist")]
        public Optional<bool> Hoist { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string> Icon { get; set; }

        [JsonPropertyName("unicode_emoji")]
        public Optional<string> UnicodeEmoji { get; set; }

        [JsonPropertyName("mentionable")]
        public Optional<bool> Mentionable { get; set; }
    }

    public record GetGuildPruneCountQuery
    {
        [JsonPropertyName("days")]
        public Optional<int> Days { get; set; }

        [JsonPropertyName("include_roles")]
        public Optional<Snowflake[]> IncludeRoles { get; set; }
    }

    public record BeginGuildPruneRequest
    {
        [JsonPropertyName("days")]
        public Optional<int> Days { get; set; }

        [JsonPropertyName("compute_prune_count")]
        public Optional<bool> ComputePruneCount { get; set; }

        [JsonPropertyName("include_roles")]
        public Optional<Snowflake[]> IncludeRoles { get; set; }

        [JsonPropertyName("reason")]
        public Optional<string> Reason { get; set; }
    }

    public record GetGuildWidgetImageQuery
    {
        [JsonPropertyName("style")]
        public Optional<string> Style { get; set; }
    }

    public record ModifyGuildWelcomeScreenRequest
    {
        [JsonPropertyName("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonPropertyName("welcome_channels")]
        public Optional<WelcomeScreenChannel[]> WelcomeChannels { get; set; }

        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }
    }

    public record ModifyGuildOnboardingRequest
    {
        [JsonPropertyName("prompts")]
        public Optional<OnboardingPrompt[]> Prompts { get; set; }

        [JsonPropertyName("default_channel_ids")]
        public Optional<Snowflake[]> DefaultChannelIds { get; set; }

        [JsonPropertyName("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonPropertyName("mode")]
        public Optional<int> Mode { get; set; }
    }

    public record ModifyGuildIncidentActionsRequest
    {
        [JsonPropertyName("invites_disabled_until")]
        public Optional<DateTimeOffset?> InvitesDisabledUntil { get; set; }

        [JsonPropertyName("dms_disabled_until")]
        public Optional<DateTimeOffset?> DmsDisabledUntil { get; set; }
    }
}
