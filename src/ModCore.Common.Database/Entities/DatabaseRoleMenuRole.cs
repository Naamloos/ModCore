using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_rolemenu_role")]
    public class DatabaseRoleMenuRole
    {
        [Column("menu_id")]
        public ulong MenuId { get; set; }

        [Column("role_id")]
        public ulong RoleId { get; set; }

        public virtual DatabaseRoleMenu Menu { get; set; }
    }
}
