using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Entities
{
    public class TimerData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TimerId")]
        [Required]
        public int TimerId { get; set; }

        [Column("TimerDispatch", TypeName = "timestamptz")]
        [Required]
        public DateTimeOffset TimerDispatch { get; set; }

        [Column("TimerType")]
        [Required]
        public TimerType TimerType { get; set; }

        [Column("Data", TypeName = "jsonb")]
        [Required]
        public string Data { get; set; }

        public T GetData<T>() =>
            JsonSerializer.Deserialize<T>(this.Data);

        public void SetSettings<T>(T data) =>
            this.Data = JsonSerializer.Serialize(data);
    }

    public enum TimerType
    {
        Reminder,
        Unmute,
        Unban
    }
}
