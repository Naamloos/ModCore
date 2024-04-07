using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_tag")]
    public class DatabaseTag
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Column("channel_id")]
        public ulong? ChannelId { get; set; } = null; // not set = global

        [Column("name")]
        [MaxLength(35)]
        public string Name { get; set; }

        [Column("author_id")]
        public ulong AuthorId { get; set; }

        [Column("content")]
        [MaxLength(255)]
        public string Content { get; set; }

        [Column("modifed_at")]
        public DateTimeOffset ModifiedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual ICollection<DatabaseTagHistory> History { get; set; } = new HashSet<DatabaseTagHistory>();
        public virtual DatabaseUser Author { get; set; }
    }
}
