using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_tags")]
    public class DatabaseTag
    {
        #region Index
        [Column("guild_id")]
        public long GuildId { get; set; } = 0;

        [Column("channel_id")]
        public long ChannelId { get; set; }

        [Column("tagname")]
        public string Name { get; set; }
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("created_at", TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; }

        [Column("contents")]
        public string Contents { get; set; }
    }
}
