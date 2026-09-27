using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildScheduledEventRecurrenceRuleNWeekday
    {
        [JsonPropertyName("n")]
        public int N { get; set; }

        [JsonPropertyName("day")]
        public int Day { get; set; }
    }
}
