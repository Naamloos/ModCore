using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Timers
{
    public record UnbanTimerData : ITimerData
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = "";
    }
}
