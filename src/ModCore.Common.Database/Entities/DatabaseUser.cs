using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_user")]
    public class DatabaseUser
    {
        [JsonPropertyName("user_id")]
        [Column("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("level_data")]
        public virtual ICollection<DatabaseLevelData> LevelData { get; set; } = new HashSet<DatabaseLevelData>();

        [JsonPropertyName("nickname_states")]
        public virtual ICollection<DatabaseNicknameState> NicknameStates { get; set; } = new HashSet<DatabaseNicknameState>();

        [JsonPropertyName("override_states")]
        public virtual ICollection<DatabaseOverrideState> OverrideStates { get; set; } = new HashSet<DatabaseOverrideState>();

        [JsonPropertyName("role_states")]
        public virtual ICollection<DatabaseRoleState> RoleStates { get; set; } = new HashSet<DatabaseRoleState>();

        [JsonPropertyName("starboard_items")]
        public virtual ICollection<DatabaseStarboardItem> StarboardItems { get; set; } = new HashSet<DatabaseStarboardItem>();

        [JsonPropertyName("starred_items")]
        public virtual ICollection<DatabaseStarboardItem> StarredItems { get; set; } = new HashSet<DatabaseStarboardItem>();

        [JsonPropertyName("tags")]
        public virtual ICollection<DatabaseTag> Tags { get; set; } = new HashSet<DatabaseTag>();

        [JsonPropertyName("ban_appeals")]
        public virtual ICollection<DatabaseBanAppeal> BanAppeals { get; set; } = new HashSet<DatabaseBanAppeal>();

        [JsonPropertyName("opened_tickets")]
        public virtual ICollection<DatabaseTicket> Tickets { get; set; } = new HashSet<DatabaseTicket>();
    }
}
