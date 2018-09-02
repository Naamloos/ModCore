using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModCore.Entities;
using ModCore.Logic.EntityFramework.AttributeImpl;
using Newtonsoft.Json;

namespace ModCore.Database
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
            JsonConvert.DeserializeObject<UserData>(this.Data);

        public void SetData(UserData data) =>
            this.Data = JsonConvert.SerializeObject(data);
    }
}
