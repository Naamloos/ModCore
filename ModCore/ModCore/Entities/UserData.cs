using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    [Table("UserData")]
    public class UserData
    {
        [Column("UserId")]
        [Required]
        public long UserId { get; set; }

        [Column("Data", TypeName = "jsonb")]
        [Required]
        public string Data { get; set; }

        public JsonUserData GetData() =>
            JsonSerializer.Deserialize<JsonUserData>(this.Data);

        public void SetSettings(JsonUserData data) =>
            this.Data = JsonSerializer.Serialize(data);
    }

    public class JsonUserData
    {

    }
}
