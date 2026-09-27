using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record OptionalAuditEntryInfo
    {
        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("auto_moderation_rule_name")]
        public string AutoModerationRuleName { get; set; } = default!;

        [JsonPropertyName("auto_moderation_rule_trigger_type")]
        public string AutoModerationRuleTriggerType { get; set; } = default!;

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("count")]
        public string Count { get; set; } = default!;

        [JsonPropertyName("delete_member_days")]
        public string DeleteMemberDays { get; set; } = default!;

        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("members_removed")]
        public string MembersRemoved { get; set; } = default!;

        [JsonPropertyName("message_id")]
        public Snowflake MessageId { get; set; }

        [JsonPropertyName("role_name")]
        public string RoleName { get; set; } = default!;

        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("integration_type")]
        public string IntegrationType { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
    }
}
