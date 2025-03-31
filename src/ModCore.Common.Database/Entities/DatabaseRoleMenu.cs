using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_rolemenu")]
    public class DatabaseRoleMenu
    {
        [JsonPropertyName("id")]
        [Column("id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long Id { get; set; }

        [JsonIgnore]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("name")]
        [Column("name")]
        [MaxLength(30)]
        public string Name { get; set; } = "";

        [JsonPropertyName("creator_id")]
        [Column("creator_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong CreatorId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonPropertyName("roles")]
        public virtual ICollection<DatabaseRoleMenuRole> Roles { get; set; } = new HashSet<DatabaseRoleMenuRole>();
    }
}
