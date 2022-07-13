using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModCore.Database.JsonEntities;
using ModCore.Utils.EntityFramework.AttributeImpl;
using Newtonsoft.Json;

namespace ModCore.Database.DatabaseEntities
{
    [Table("mcore_userdata")]
    public class DatabaseUserData
    {
        #region index
        [Index("user_id_key", IsUnique = true, IsLocal = true)]
        [Column("user_id")]
        public long UserId { get; set; }
        #endregion

        [Column("id")]
        public int Id { get; set; }

        [Column("usr_data", TypeName = "jsonb")]
        [Required]
        public string Data { get; set; } = "";

        public UserData GetData() =>
            JsonConvert.DeserializeObject<UserData>(Data);

        public void SetData(UserData data) =>
            Data = JsonConvert.SerializeObject(data);
    }
}
