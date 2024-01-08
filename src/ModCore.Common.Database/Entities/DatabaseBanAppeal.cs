using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_appeal")]
    public class DatabaseBanAppeal
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("user_id")]
        public ulong UserId { get; set; }

        [Column("appeal_content")]
        public string AppealContent {  get; set; }

        public virtual DatabaseUser User { get; set; }
        public virtual DatabaseGuild Guild { get; set; }
    }
}
