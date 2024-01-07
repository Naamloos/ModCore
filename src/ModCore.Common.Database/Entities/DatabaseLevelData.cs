using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_leveldata")]
    public class DatabaseLevelData
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("user_id")]
        public ulong UserId { get; set; }

        [Column("experience")]
        public long Experience { get; set; }

        [Column("last_xp_grant")]
        public DateTimeOffset LastGrant { get; set; }

        public DatabaseGuild Guild { get; set; }
    }
}
