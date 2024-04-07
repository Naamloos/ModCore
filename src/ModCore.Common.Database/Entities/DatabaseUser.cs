using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_user")]
    public class DatabaseUser
    {
        [Column("user_id")]
        public ulong UserId { get; set; }

        public virtual ICollection<DatabaseLevelData> LevelData { get; set; } = new HashSet<DatabaseLevelData>();
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; } = new HashSet<DatabaseNicknameState>();
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; } = new HashSet<DatabaseOverrideState>();
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; } = new HashSet<DatabaseRoleState>();
        public virtual ICollection<DatabaseStarboardItem> StarboardItems { get; set; } = new HashSet<DatabaseStarboardItem>();
        public virtual ICollection<DatabaseTag> Tags { get; set; } = new HashSet<DatabaseTag>();
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; } = new HashSet<DatabaseBanAppeal>();
        public virtual ICollection<DatabaseTicket> Tickets { get; set; } = new HashSet<DatabaseTicket>();
    }
}
