using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_rolestate_roles")]
    public class DatabaseRolestateRoles
    {
        #region Index
        [Column("member_id")]
        public long MemberId { get; set; }

        [Column("guild_id")]
        public long GuildId { get; set; }
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("role_ids")]
        public long[] RoleIds { get; set; }
    }
}
