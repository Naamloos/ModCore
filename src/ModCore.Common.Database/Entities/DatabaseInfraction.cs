using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_infraction")]
    public class DatabaseInfraction
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("user_id")]
        public ulong UserId { get; set; }

        [Column("responsible_moderator_id")]
        public ulong ResponsibleModerator {  get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("user_was_notified")]
        public bool UserNotified { get; set; }

        [Column("infraction_type")]
        public InfractionType Type { get; set; }

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
