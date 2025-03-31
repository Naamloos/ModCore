using Microsoft.EntityFrameworkCore;
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
    [Table("mcore_leveldata")]
    public class DatabaseLevelData
    {
        [JsonPropertyName("guild_id")]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("user_id")]
        [Column("user_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("experience")]
        [Column("experience")]
        public long Experience { get; set; }

        [JsonPropertyName("last_xp_grant")]
        [Column("last_xp_grant")]
        public DateTimeOffset LastGrant { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser User { get; set; }
    }
}
