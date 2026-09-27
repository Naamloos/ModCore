using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record IncidentsData
    {
        [JsonPropertyName("invites_disabled_until")]
        public DateTimeOffset? InvitesDisabledUntil { get; set; }

        [JsonPropertyName("dms_disabled_until")]
        public DateTimeOffset? DmsDisabledUntil { get; set; }

        [JsonPropertyName("dm_spam_detected_at")]
        public Optional<DateTimeOffset?> DmSpamDetectedAt { get; set; }

        [JsonPropertyName("raid_detected_at")]
        public Optional<DateTimeOffset?> RaidDetectedAt { get; set; }
    }
}
