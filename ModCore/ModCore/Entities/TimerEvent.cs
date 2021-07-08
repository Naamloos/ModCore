using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    public class TimerEvent
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        [Column("timer_type", TypeName = "integer")]
        public TimerType Type { get; set; }

        [Column("guild_id")]
        public long GuildId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("channel_id")]
        public long ChannelId { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("dispatch", TypeName = "timestamptz")]
        public DateTimeOffset Dispatch { get; set; }

        [Column("creation", TypeName = "timestamptz")]
        public DateTimeOffset Creation { get; set; }
    }


    public enum TimerType
    {
        Unknown = 0, // Action type that is not known
        Reminder = 1, // Reminders
        Unban = 2, // Temp ban unban action
        Unmute = 3, // Temp mute unmute action
    }
}
