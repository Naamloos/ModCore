using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities
{
    [Table("mcore_stars")]
    public class DatabaseStarData
    {
        #region Index
        [Column("message_id")]
        public long MessageId { get; set; } // Message Id

        [Column("channel_id")]
        public long ChannelId { get; set; } // Channel this was sent in

        [Column("stargazer_id")]
        public long StargazerId { get; set; } // Member that starred
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("starboard_entry_id")]
        public long StarboardMessageId { get; set; } // Id for starboard entry message

        [Column("author_id")]
        public long AuthorId { get; set; } // Author Id

        [Column("guild_id")]
        public long GuildId { get; set; } // Guild this belongs to
    }
}
