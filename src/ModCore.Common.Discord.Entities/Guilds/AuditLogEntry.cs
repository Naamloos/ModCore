using ModCore.Common.Discord.Entities.Enums;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents a single entry in a Discord guild audit log.
    /// </summary>
    public record AuditLogEntry
    {
        /// <summary>
        /// Gets or sets the ID of the entity targeted by this audit log entry.
        /// </summary>
        /// <remarks>
        /// This may refer to a guild, channel, user, role, webhook, emoji, sticker, invite,
        /// scheduled event, thread, integration, or another Discord entity depending on
        /// <see cref="ActionType"/>.
        /// </remarks>
        [JsonPropertyName("target_id")]
        public string TargetId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of changes made by this audit log entry.
        /// </summary>
        /// <remarks>
        /// This value may be absent depending on the audit log action type.
        /// </remarks>
        [JsonPropertyName("changes")]
        public Optional<AuditLogChange[]> Changes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who performed the action.
        /// </summary>
        /// <remarks>
        /// This value may be <see langword="null"/> for some system-generated audit log entries.
        /// </remarks>
        [JsonPropertyName("user_id")]
        public Snowflake? UserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ID of this audit log entry.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; } = 0;

        /// <summary>
        /// Gets or sets the type of action represented by this audit log entry.
        /// </summary>
        [JsonPropertyName("action_type")]
        public AuditLogEvent ActionType { get; set; }

        /// <summary>
        /// Gets or sets additional metadata for this audit log entry.
        /// </summary>
        /// <remarks>
        /// The available option fields depend on <see cref="ActionType"/>.
        /// This value may be absent for action types that do not include additional options.
        /// </remarks>
        [JsonPropertyName("options")]
        public Optional<AuditLogEntryOptions> Options { get; set; }

        /// <summary>
        /// Gets or sets the reason provided for this audit log action.
        /// </summary>
        /// <remarks>
        /// This value is only present when a reason was supplied with the audited action.
        /// </remarks>
        [JsonPropertyName("reason")]
        public Optional<string> Reason { get; set; }
    }

    /// <summary>
    /// Represents a single field-level change in a Discord audit log entry.
    /// </summary>
    public record AuditLogChange
    {
        /// <summary>
        /// Gets or sets the new value of the changed field.
        /// </summary>
        /// <remarks>
        /// The JSON value type depends on <see cref="Key"/>.
        /// If this value is absent while <see cref="OldValue"/> is present, the property may have been reset or removed.
        /// </remarks>
        [JsonPropertyName("new_value")]
        public Optional<JsonElement> NewValue { get; set; }

        /// <summary>
        /// Gets or sets the previous value of the changed field.
        /// </summary>
        /// <remarks>
        /// The JSON value type depends on <see cref="Key"/>.
        /// If this value is absent while <see cref="NewValue"/> is present, the property may not have had a previous value.
        /// </remarks>
        [JsonPropertyName("old_value")]
        public Optional<JsonElement> OldValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the changed audit log field.
        /// </summary>
        /// <remarks>
        /// This value determines the meaning and expected JSON type of <see cref="OldValue"/> and <see cref="NewValue"/>.
        /// </remarks>
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents additional metadata for specific Discord audit log entry types.
    /// </summary>
    /// <remarks>
    /// Not every property is available for every audit log action.
    /// The populated fields depend on the parent audit log entry's action type.
    /// </remarks>
    public record AuditLogEntryOptions
    {
        /// <summary>
        /// Gets the ID of the application related to the audit log action.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; init; }

        /// <summary>
        /// Gets the name of the auto moderation rule related to the audit log action.
        /// </summary>
        [JsonPropertyName("auto_moderation_rule_name")]
        public Optional<string> AutoModerationRuleName { get; init; }

        /// <summary>
        /// Gets the trigger type of the auto moderation rule related to the audit log action.
        /// </summary>
        /// <remarks>
        /// Discord serializes this value as a string in audit log options.
        /// </remarks>
        [JsonPropertyName("auto_moderation_rule_trigger_type")]
        public Optional<string> AutoModerationRuleTriggerType { get; init; }

        /// <summary>
        /// Gets the ID of the channel related to the audit log action.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; init; }

        /// <summary>
        /// Gets the number of affected entities for the audit log action.
        /// </summary>
        /// <remarks>
        /// Discord serializes this value as a string.
        /// </remarks>
        [JsonPropertyName("count")]
        public Optional<string> Count { get; init; }

        /// <summary>
        /// Gets the number of days after which inactive members were included in a prune action.
        /// </summary>
        /// <remarks>
        /// Discord serializes this value as a string.
        /// </remarks>
        [JsonPropertyName("delete_member_days")]
        public Optional<string> DeleteMemberDays { get; init; }

        /// <summary>
        /// Gets the ID of the overwritten entity, affected entity, or related option entity.
        /// </summary>
        /// <remarks>
        /// The meaning of this value depends on the audit log action type.
        /// For channel permission overwrite actions, this is the ID of the role or member overwrite.
        /// </remarks>
        [JsonPropertyName("id")]
        public Optional<Snowflake> Id { get; init; }

        /// <summary>
        /// Gets the number of members removed by a prune action.
        /// </summary>
        /// <remarks>
        /// Discord serializes this value as a string.
        /// </remarks>
        [JsonPropertyName("members_removed")]
        public Optional<string> MembersRemoved { get; init; }

        /// <summary>
        /// Gets the ID of the message related to the audit log action.
        /// </summary>
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; init; }

        /// <summary>
        /// Gets the name of the role related to the audit log action.
        /// </summary>
        /// <remarks>
        /// This is commonly used for channel permission overwrite audit log entries.
        /// </remarks>
        [JsonPropertyName("role_name")]
        public Optional<string> RoleName { get; init; }

        /// <summary>
        /// Gets the type of overwritten entity or related option type.
        /// </summary>
        /// <remarks>
        /// For channel permission overwrite actions, Discord uses <c>"0"</c> for a role overwrite
        /// and <c>"1"</c> for a member overwrite.
        /// Discord serializes this value as a string.
        /// </remarks>
        [JsonPropertyName("type")]
        public Optional<string> Type { get; init; }

        /// <summary>
        /// Gets the type of integration related to the audit log action.
        /// </summary>
        [JsonPropertyName("integration_type")]
        public Optional<string> IntegrationType { get; init; }

        /// <summary>
        /// Gets the status related to the audit log action.
        /// </summary>
        /// <remarks>
        /// The meaning of this value depends on the audit log action type.
        /// </remarks>
        [JsonPropertyName("status")]
        public Optional<string> Status { get; init; }

        /// <summary>
        /// Gets additional option fields returned by Discord that are not explicitly modeled.
        /// </summary>
        /// <remarks>
        /// This allows the model to remain forward-compatible when Discord adds new audit log option fields.
        /// </remarks>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? UnknownFields { get; init; }
    }
}