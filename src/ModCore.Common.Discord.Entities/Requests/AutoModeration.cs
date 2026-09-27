using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateAutoModerationRuleRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("event_type")]
        public Optional<int> EventType { get; set; }

        [JsonPropertyName("trigger_type")]
        public Optional<int> TriggerType { get; set; }

        [JsonPropertyName("trigger_metadata")]
        public Optional<TriggerMetadata> TriggerMetadata { get; set; }

        [JsonPropertyName("actions")]
        public Optional<AutoModerationAction[]> Actions { get; set; }

        [JsonPropertyName("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonPropertyName("exempt_roles")]
        public Optional<Snowflake[]> ExemptRoles { get; set; }

        [JsonPropertyName("exempt_channels")]
        public Optional<Snowflake[]> ExemptChannels { get; set; }
    }

    public record ModifyAutoModerationRuleRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("event_type")]
        public Optional<int> EventType { get; set; }

        [JsonPropertyName("trigger_metadata")]
        public Optional<TriggerMetadata> TriggerMetadata { get; set; }

        [JsonPropertyName("actions")]
        public Optional<AutoModerationAction[]> Actions { get; set; }

        [JsonPropertyName("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonPropertyName("exempt_roles")]
        public Optional<Snowflake[]> ExemptRoles { get; set; }

        [JsonPropertyName("exempt_channels")]
        public Optional<Snowflake[]> ExemptChannels { get; set; }
    }
}
