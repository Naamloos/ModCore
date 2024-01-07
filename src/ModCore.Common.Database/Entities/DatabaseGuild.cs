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
    [Table("mcore_guild")]
    public class DatabaseGuild
    {
        [Column("guild_id")]
        public ulong Id { get; set; }

        [Column("logging_channel_id")]
        public ulong LoggingChannel {  get; set; }

        public virtual ICollection<DatabaseLevelData> LevelData { get; set; }
    }
}
