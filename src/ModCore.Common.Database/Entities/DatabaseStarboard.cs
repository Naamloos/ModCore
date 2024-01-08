using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_starboard")]
    public class DatabaseStarboard
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("enabled")]
        public bool Enabled { get; set; }

        [Column("minimum_reactions")]
        public int MinimumReactions { get; set; } = 3;

        [Column("emoji")]
        public string Emoji { get; set; } = "⭐";

        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        public virtual DatabaseGuild Guild { get; set; }
        public virtual ICollection<DatabaseStarboardItem> Items { get; set; }
    }
}
