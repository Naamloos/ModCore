using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_autorole")]
    public class DatabaseAutoRole
    {
        [JsonIgnore]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("role_id")]
        [Column("role_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong RoleId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
    }
}
