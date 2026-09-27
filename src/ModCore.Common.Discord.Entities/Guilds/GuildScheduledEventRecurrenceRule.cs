using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record GuildScheduledEventRecurrenceRule
    {
        [JsonPropertyName("start")]
        public DateTimeOffset Start { get; set; }

        [JsonPropertyName("end")]
        public DateTimeOffset? End { get; set; }

        [JsonPropertyName("frequency")]
        public int Frequency { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        [JsonPropertyName("by_weekday")]
        public int[]? ByWeekday { get; set; }

        [JsonPropertyName("by_n_weekday")]
        public GuildScheduledEventRecurrenceRuleNWeekday[]? ByNWeekday { get; set; }

        [JsonPropertyName("by_month")]
        public int[]? ByMonth { get; set; }

        [JsonPropertyName("by_month_day")]
        public int[]? ByMonthDay { get; set; }

        [JsonPropertyName("by_year_day")]
        public int[]? ByYearDay { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }
    }
}
