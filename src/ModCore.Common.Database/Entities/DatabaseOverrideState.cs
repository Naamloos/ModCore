using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_override_state")]
    public class DatabaseOverrideState
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("user_id")]
        public ulong UserId { get; set; }

        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [Column("allowed")]
        public long AllowedPermissions { get; set; }

        [Column("denied")]
        public long DeniedPermissions { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual DatabaseUser User { get; set; }
    }
}
