using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_rolemenu")]
    public class DatabaseRoleMenu
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("name")]
        [MaxLength(30)]
        public ulong Name { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual ICollection<DatabaseRoleMenuRole> Roles { get; set; }
    }
}
