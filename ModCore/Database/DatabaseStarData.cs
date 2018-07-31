using System.ComponentModel.DataAnnotations.Schema;
using ModCore.Logic.EntityFramework.AttributeImpl;

namespace ModCore.Database
{
    [Table("mcore_stars")]
    public class DatabaseStarData
    {
        #region Index
        [Index("member_id_guild_id_key", IsUnique = true, IsLocal = true)]
        [Column("stargazer_id")]
        public long StargazerId { get; set; } // Member that starred
        
        [Index("member_id_guild_id_key", IsUnique = true, IsLocal = true)]
        [Column("message_id")]
        public long MessageId { get; set; } // Message Id
        
        [Index("member_id_guild_id_key", IsUnique = true, IsLocal = true)]
        [Column("channel_id")]
        public long ChannelId { get; set; } // Channel this was sent in
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