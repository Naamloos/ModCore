using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public class CurrentUserGuild : Guild
    {
        [JsonPropertyName("owner")]
        public bool IsOwner { get; set; }

        [JsonPropertyName("permissions")]
        public string? Permissions { get; set; }
    }

    public class Guild
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("icon_hash")]
        public Optional<string?> IconHash { get; set; }

        [JsonPropertyName("splash")]
        public string? Splash { get; set; }

        [JsonPropertyName("discovery_splash")]
        public string? DiscoverySplash { get; set; }

        [JsonPropertyName("owner_id")]
        public Snowflake OwnerId { get; set; }

        [JsonPropertyName("afk_channel_id")]
        public Snowflake? AfkChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        public int AfkTimeout { get; set; }

        [JsonPropertyName("widget_enabled")]
        public Optional<bool> WidgetEnabled { get; set; }

        [JsonPropertyName("widget_channel_id")]
        public Optional<Snowflake?> WidgetChannelId { get; set; }

        [JsonPropertyName("verification_level")]
        public int VerificationLevel { get; set; }

        [JsonPropertyName("default_message_notifications")]
        public int DefaultMessageNotifications { get; set; }

        [JsonPropertyName("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }

        [JsonPropertyName("roles")]
        public Role[] Roles { get; set; }

        [JsonPropertyName("emojis")]
        public Emoji[] Emojis { get; set; }

        [JsonPropertyName("features")]
        public string[] Features { get; set; }

        [JsonPropertyName("mfa_level")]
        public int MFALevel { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; set; }

        [JsonPropertyName("system_channel_id")]
        public Snowflake? SystemChannelId { get; set; }

        [JsonPropertyName("system_channel_flags")]
        public int SystemChannelFlags { get; set; }

        [JsonPropertyName("rules_channel_id")]
        public Snowflake? RulesChannelId { get; set; }

        [JsonPropertyName("max_members")]
        public Optional<int> MaxMembers { get; set; }

        [JsonPropertyName("vanity_url_code")]
        public string? VanityUrlCode { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("premium_tier")]
        public BoostTier PremiumTier { get; set; }

        [JsonPropertyName("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; set; }

        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }

        [JsonPropertyName("public_updates_channel_id")]
        public Snowflake? PublicUpdatesChannelId { get; set; }

        [JsonPropertyName("max_video_channel_users")]
        public Optional<int> MaxVideoChannelUsers { get; set; }

        [JsonPropertyName("max_stage_video_channel_users")]
        public Optional<int> MaxStageVideoChannelUsers { get; set; }

        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }

        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }

        [JsonPropertyName("welcome_screen")]
        public Optional<WelcomeScreen> WelcomeScreen { get; set; }

        [JsonPropertyName("nsfw_level")]
        public int NSFWLevel { get; set; }

        [JsonPropertyName("stickers")]
        public Optional<Sticker[]> Stickers { get; set; }

        [JsonPropertyName("premium_progress_bar_enabled")]
        public bool PremiumProgressBarEnabled { get; set; }

        [JsonPropertyName("safety_alerts_channel_id")]
        public Snowflake? SafetyAlertsChannelId { get; set; }
    }

    public class WelcomeScreen
    {
    }

    public enum BoostTier
    {
        None = 0,
        Tier1 = 1,
        Tier2 = 2,
        Tier3 = 3
    }
}
