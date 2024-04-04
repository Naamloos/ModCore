using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_levels")]
    public class DatabaseLevel
    {
        #region Index
        [Column("channel_id")]
        public long GuildId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }
        #endregion

        /// <summary>
        /// Ignore me. Indexed by guild/user.
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        [Column("experience")]
        public int Experience { get; set; }

        [Column("last_xp_grant", TypeName = "timestamptz")]
        public DateTime LastXpGrant { get; set; }
    }
}
