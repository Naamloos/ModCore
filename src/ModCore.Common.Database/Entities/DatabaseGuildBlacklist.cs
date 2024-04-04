using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    public class DatabaseGuildBlacklist
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("block_reason")]
        public ulong BlockReason { get; set; }
    }
}
