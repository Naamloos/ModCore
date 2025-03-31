using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_role_state")]
    public class DatabaseRoleState
    {
        [JsonIgnore]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("role_id")]
        [Column("role_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong RoleId { get; set; }

        [JsonPropertyName("user_id")]
        [Column("user_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong UserId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser User { get; set; }
    }
}
