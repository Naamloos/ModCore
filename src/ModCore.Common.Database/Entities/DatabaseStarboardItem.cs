using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_starboard_item")]
    public class DatabaseStarboardItem
    {
        [Column("starboard_id")]
        public long StarboardId { get; set; }

        [Column("message_id")]
        public ulong MessageId { get; set; }

        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [Column("board_message_id")]
        public ulong BoardMessageId { get; set; }

        [Column("author_id")]
        public ulong AuthorId { get; set; }

        [Column("star_amount")]
        public ulong StarAmount { get; set; }

        public virtual DatabaseStarboard Starboard { get; set; }
        public virtual DatabaseUser Author { get; set; }
    }
}
