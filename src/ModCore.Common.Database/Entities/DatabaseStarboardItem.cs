using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_starboard_item")]
    public class DatabaseStarboardItem
    {
        [JsonPropertyName("starboard_id")]
        [Column("starboard_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long StarboardId { get; set; }

        [JsonPropertyName("message_id")]
        [Column("message_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong MessageId { get; set; }

        [JsonPropertyName("channel_id")]
        [Column("channel_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("board_message_id")]
        [Column("board_message_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong BoardMessageId { get; set; }

        [JsonPropertyName("author_id")]
        [Column("author_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong AuthorId { get; set; }

        [JsonPropertyName("stargazer_id")]
        [Column("stargazer_id")] // Member that starred
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong StargazerId { get; set; }

        [JsonIgnore]
        public virtual DatabaseStarboard Starboard { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser Author { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser Stargazer { get; set; }
    }
}
