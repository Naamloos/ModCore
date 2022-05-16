using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModCore.Logic.EntityFramework.AttributeImpl;

namespace ModCore.Database
{
    [Table("mcore_tags")]
    public class DatabaseTag
    {
        #region Index
        [Index("guild_id_channel_id_name_key", IsUnique = true, IsLocal = true)]
        [Column("guild_id")]
        public long GuildId { get; set; } = 0;

        [Index("guild_id_channel_id_name_key", IsUnique = true, IsLocal = true)]
        [Column("channel_id")]
        public long ChannelId { get; set; }
        
        [Index("guild_id_channel_id_name_key", IsUnique = true, IsLocal = true)]
        [Column("tagname")]
        public string Name { get; set; }
        #endregion
        
        [Column("id")]
        public int Id { get; set; }
        
        [Column("owner_id")]
        public long OwnerId { get; set; }
        
        [Column("created_at", TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; }
        
        [Column("contents")]
        public string Contents { get; set; }
    }
}
