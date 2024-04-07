using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Entities
{
    [Table("mcore_welcomer")]
    public class DatabaseWelcomeSettings
    {
        [Column("guild_id")]
        public ulong GuildId { get; set; }

        [Column("channel_id")]
        public ulong ChannelId { get; set; }

        [Column("message_id")]
        public string Message { get; set; }

        [Column("image_b64")]
        public string? ImageB64 { get; set; }

        [Column("x")]
        public int ImageX { get; set; }

        [Column("y")]
        public int ImageY { get; set; }

        [Column("width")]
        public int ImageWidth { get; set; }

        [Column("height")]
        public int ImageHeight { get; set; }

        [Column("shape")]
        public WelcomeImageShape Shape { get; set; }

        [Column("enabled")]
        public bool Enabled = false;

        public DatabaseGuild Guild { get; set; }
    }

    public enum WelcomeImageShape
    {
        Square = 0,
        Circle = 1,
        Squircle = 2 
    }
}
