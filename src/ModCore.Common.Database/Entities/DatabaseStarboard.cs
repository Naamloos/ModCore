using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_starboard")]
    public class DatabaseStarboard
    {
        [JsonPropertyName("id")]
        [Column("id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long Id { get; set; }

        [JsonIgnore]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("enabled")]
        [Column("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("minimum_reactions")]
        [Column("minimum_reactions")]
        public int MinimumReactions { get; set; } = 3;

        [JsonPropertyName("emoji")]
        [Column("emoji")]
        public string Emoji { get; set; } = "⭐";

        [JsonPropertyName("channel_id")]
        [Column("channel_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual ICollection<DatabaseStarboardItem> Items { get; set; } = new HashSet<DatabaseStarboardItem>();
    }
}
