using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_infraction")]
    public class DatabaseInfraction
    {
        [JsonIgnore]
        [Column("id")]
        public long Id { get; set; }

        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("user_id")]
        [Column("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("responsible_moderator_id")]
        [Column("responsible_moderator_id")]
        public ulong ResponsibleModerator {  get; set; }

        [JsonPropertyName("reason")]
        [Column("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("user_was_notified")]
        [Column("user_was_notified")]
        public bool UserNotified { get; set; }

        [JsonPropertyName("infraction_type")]
        [Column("infraction_type")]
        public InfractionType Type { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
    }

    public enum InfractionType
    {
        Warning,
        Ban,
        Kick,
        Mute,
        TempBan,
        SoftBan,
        HackBan,
        MassBan,
        Isolate,
        Appealed,
        AppealDenied,
        VoiceBan
    }
}
