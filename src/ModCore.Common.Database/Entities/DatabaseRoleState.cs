using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_role_state")]
    public class DatabaseRoleState
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("role_id")]
        public ulong RoleId { get; set; }

        [Column("user_id")]
        public ulong UserId { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual DatabaseUser User { get; set; }
    }
}
