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
    [Table("mcore_tag")]
    public class DatabaseTag
    {
        [JsonPropertyName("id")]
        [Column("id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long Id { get; set; }

        [JsonPropertyName("channel_id")]
        [Column("channel_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong? ChannelId { get; set; } = null; // not set = global

        [JsonPropertyName("name")]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [JsonPropertyName("author_id")]
        [Column("author_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong AuthorId { get; set; }

        [JsonPropertyName("content")]
        [Column("content")]
        [MaxLength(2000)]
        public string Content { get; set; }

        [JsonPropertyName("modified_at")]
        [Column("modifed_at")]
        public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("created_at")]
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("guild_id")]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonPropertyName("history")]
        public virtual ICollection<DatabaseTagHistory> History { get; set; } = new HashSet<DatabaseTagHistory>();
        [JsonIgnore]
        public virtual DatabaseUser Author { get; set; }
    }
}
