using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_override_state")]
    public class DatabaseOverrideState
    {
        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("user_id")]
        [Column("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("channel_id")]
        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("allowed")]
        [Column("allowed")]
        public long AllowedPermissions { get; set; }

        [JsonPropertyName("denied")]
        [Column("denied")]
        public long DeniedPermissions { get; set; }

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
        [JsonIgnore]
        public virtual DatabaseUser User { get; set; }
    }
}
