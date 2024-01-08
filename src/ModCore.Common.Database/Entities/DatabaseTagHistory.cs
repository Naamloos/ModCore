using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_tag_history")]
    public class DatabaseTagHistory
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("tag_id")]
        public ulong TagId { get; set; }

        [Column("content")]
        [MaxLength(255)]
        public string Content { get; set; }

        [Column("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        public virtual DatabaseTag Tag { get; set; }
    }
}
