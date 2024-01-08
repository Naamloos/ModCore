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

        public virtual ICollection<DatabaseLevelData> LevelData { get; set; }
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; }
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; }
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; }
        public virtual ICollection<DatabaseStarboardItem> StarboardItems { get; set; }
        public virtual ICollection<DatabaseTag> Tags { get; set; }
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; }
        public virtual ICollection<DatabaseTicket> Tickets { get; set; }
    }
}
