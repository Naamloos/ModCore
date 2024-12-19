using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_appeal")]
    public class DatabaseBanAppeal
    {
        [JsonPropertyName("guild_id")]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("user_id")]
        [Column("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("appeal_content")]
        [Column("appeal_content")]
        public string AppealContent {  get; set; }

        [JsonIgnore]
        public virtual DatabaseUser User { get; set; }
        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
    }
}
