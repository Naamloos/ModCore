using ModCore.Common.Database.Timers;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ModCore.Common.Utils;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_welcomer")]
    public class DatabaseWelcomeSettings
    {
        [JsonIgnore]
        [Column("guild_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("welcome_message_json")]
        [Column("welcome_message_json")]
        [MaxLength(100000)]
        public string WelcomeMessageJson { get; set; }

        [JsonPropertyName("enabled")]
        [Column("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }

        public T GetData<T>() where T : CreateMessage
            => JsonSerializer.Deserialize<T>(WelcomeMessageJson, options: JsonSerializerOptionsFactory.GetOptions())!;

        public void SetData<T>(T data) where T : CreateMessage
            => WelcomeMessageJson = JsonSerializer.Serialize(data, options: JsonSerializerOptionsFactory.GetOptions());
    }
}
