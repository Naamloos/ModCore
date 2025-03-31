using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_rolemenu_role")]
    public class DatabaseRoleMenuRole
    {
        [JsonIgnore]
        [Column("menu_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long MenuId { get; set; }

        [JsonPropertyName("role_id")]
        [Column("role_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong RoleId { get; set; }

        [JsonIgnore]
        public virtual DatabaseRoleMenu Menu { get; set; }
    }
}
