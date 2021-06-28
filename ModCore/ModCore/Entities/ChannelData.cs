using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    [Table("ChannelData")]
    public class ChannelData
    {
        [Column("ChannelId")]
        [Required]
        public long ChannelId { get; set; }

        [Column("GuildId")]
        [Required]
        public long GuildId { get; set; }

        [Column("Data", TypeName = "jsonb")]
        [Required]
        public string Data { get; set; }

        public JsonChannelData GetData() =>
            JsonSerializer.Deserialize<JsonChannelData>(this.Data);

        public void SetSettings(JsonChannelData data) =>
            this.Data = JsonSerializer.Serialize(data);
    }

    public class JsonChannelData
    {

    }
}
