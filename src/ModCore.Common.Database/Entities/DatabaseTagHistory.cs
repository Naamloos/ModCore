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
    [Table("mcore_tag_history")]
    public class DatabaseTagHistory
    {
        [JsonIgnore]
        [Column("id")]
        public long Id { get; set; }

        [JsonIgnore]
        [Column("tag_id")]
        public long TagId { get; set; }

        [JsonPropertyName("content")]
        [Column("content")]
        [MaxLength(255)]
        public string Content { get; set; }

        [JsonPropertyName("timestamp")]
        [Column("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonIgnore]
        public virtual DatabaseTag Tag { get; set; }
    }
}
