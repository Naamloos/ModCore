using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_welcomer")]
    public class DatabaseWelcomeSettings
    {
        [JsonIgnore]
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("message_id")]
        [Column("message_id")]
        public string Message { get; set; } = "";

        [JsonPropertyName("image_b64")]
        [Column("image_b64")]
        public string? ImageB64 { get; set; }

        [JsonPropertyName("x")]
        [Column("x")]
        public int ImageX { get; set; }

        [JsonPropertyName("y")]
        [Column("y")]
        public int ImageY { get; set; }

        [JsonPropertyName("width")]
        [Column("width")]
        public int ImageWidth { get; set; }

        [JsonPropertyName("height")]
        [Column("height")]
        public int ImageHeight { get; set; }

        [JsonPropertyName("shape")]
        [Column("shape")]
        public WelcomeImageShape Shape { get; set; }

        [JsonPropertyName("enabled")]
        [Column("enabled")]
        public bool Enabled = false;

        [JsonIgnore]
        public virtual DatabaseGuild Guild { get; set; }
    }

    public enum WelcomeImageShape
    {
        Square = 0,
        Circle = 1,
        Squircle = 2 
    }
}
