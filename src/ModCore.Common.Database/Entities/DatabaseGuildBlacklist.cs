using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    public class DatabaseGuildBlacklist
    {
        [JsonPropertyName("guild_id")]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("block_reason")]
        [Column("block_reason")]
        public ulong BlockReason { get; set; }
    }
}
