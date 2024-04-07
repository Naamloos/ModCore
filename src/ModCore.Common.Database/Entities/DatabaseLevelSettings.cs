using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    public class DatabaseLevelSettings
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("levels_enabled")]
        public bool Enabled { get; set; } = false;

        [Column("messages_enabled")]
        public bool MessagesEnabled { get; set; } = false;

        [Column("redirect_messages")]
        public bool RedirectMessages { get; set; } = false;

        [Column("message_channel_id")]
        public ulong ChannelId { get; set; } = 0;

        public virtual DatabaseGuild Guild { get; set; }
    }
}
