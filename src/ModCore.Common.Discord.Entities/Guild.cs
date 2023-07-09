using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public string Icon { get; set; }

        [JsonPropertyName("icon_hash")]
        public string IconHash { get; set; }

        [JsonPropertyName("splash")]
        public string Splash { get; set; }

        [JsonPropertyName("owner_id")]
        public Snowflake OwnerId { get; set; }

        [JsonPropertyName("afk_channel_id")]
        public Snowflake? AfkChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        public int AfkTimeout { get; set; }

        [JsonPropertyName("widget_enabled")]
        public bool WidgetEnabled { get; set; }

        [JsonPropertyName("widget_channel_id")]
        public Snowflake? WidgetChannelId { get; set; }

        [JsonPropertyName("verification_level")]
        public int VerificationLevel { get; set; }

        [JsonPropertyName("default_message_notifications")]
        public int DefaultMessageNotifications { get; set; }

        [JsonPropertyName("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }

        [JsonPropertyName("roles")]
        public JsonObject[] Roles { get; set; }

        [JsonPropertyName("emojis")]
        public JsonObject[] Emojis { get; set; }

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

        [JsonPropertyName("vanity_url_code")]
        public string? VanityUrlCode { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }

        [JsonPropertyName("public_updates_channel_id")]
        public Snowflake? PublicUpdatesChannelId { get; set; }

        [JsonPropertyName("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }

        [JsonPropertyName("nsfw_level")]
        public int NSFWLevel { get; set; }

        [JsonPropertyName("stickers")]
        public JsonObject[] Stickers { get; set; }
    }
}
