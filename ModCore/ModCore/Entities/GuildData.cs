using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    [Table("GuildData")]
    public class GuildData
    {
        [Column("GuildId")]
        [Required]
        public long GuildId { get; set; }

        [Column("Data", TypeName = "jsonb")]
        [Required]
        public string Data { get; set; }

        public JsonGuildData GetData() =>
            JsonSerializer.Deserialize<JsonGuildData>(this.Data);

        public void SetSettings(JsonGuildData data) =>
            this.Data = JsonSerializer.Serialize(data);
    }

    public class JsonGuildData
    {

    }
}
