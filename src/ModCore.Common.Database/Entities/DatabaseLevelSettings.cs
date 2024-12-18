using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    public class DatabaseLevelSettings
    {
        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("enabled")]
        [Column("levels_enabled")]
        public bool Enabled { get; set; } = false;

        [JsonPropertyName("messages_enabled")]
        [Column("messages_enabled")]
        public bool MessagesEnabled { get; set; } = false;

        [JsonPropertyName("redirect_messages")]
        [Column("redirect_messages")]
        public bool RedirectMessages { get; set; } = false;

        [JsonPropertyName("message_channel_id")]
        [Column("message_channel_id")]
        public ulong ChannelId { get; set; } = 0;

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
    }
}
