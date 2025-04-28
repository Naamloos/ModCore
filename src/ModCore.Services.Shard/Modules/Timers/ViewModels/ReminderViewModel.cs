using ModCore.Common.Database.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Modules.Timers.ViewModels
{
    public class ReminderViewModel
    {
        public int AccentColor { get; init; } = 0x089FDF;

        public string Message { get; set; } = "";

        public string PingMessage { get; set; } = "";

        public ReminderViewModel(ReminderTimerData timerData)
        {
            Message = timerData.Text;
            var now = DateTimeOffset.UtcNow;
            PingMessage = $"## ⏰ Hey, <@{timerData.UserId}>!\nAt <t:{timerData.CreatedAt.ToUnixTimeSeconds()}:f> you set a reminder to trigger <t:{now.ToUnixTimeSeconds()}:R>!";
        }
    }
}
