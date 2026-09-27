using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record TriggerMetadata
    {
        [JsonPropertyName("keyword_filter")]
        public Optional<string[]> KeywordFilter { get; set; }

        [JsonPropertyName("regex_patterns")]
        public Optional<string[]> RegexPatterns { get; set; }

        [JsonPropertyName("presets")]
        public Optional<int[]> Presets { get; set; }

        [JsonPropertyName("allow_list")]
        public Optional<string[]> AllowList { get; set; }

        [JsonPropertyName("mention_total_limit")]
        public Optional<int> MentionTotalLimit { get; set; }

        [JsonPropertyName("mention_raid_protection_enabled")]
        public Optional<bool> MentionRaidProtectionEnabled { get; set; }
    }
}
