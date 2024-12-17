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
        public long Id { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("name")]
        [Column("name")]
        [MaxLength(30)]
        public string Name { get; set; }

        [JsonPropertyName("creator_id")]
        [Column("creator_id")]
        public ulong CreatorId { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        [JsonPropertyName("roles")]
        public virtual ICollection<DatabaseRoleMenuRole> Roles { get; set; } = new HashSet<DatabaseRoleMenuRole>();
    }
}
