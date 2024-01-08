﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Timers
{
    public record ReminderTimerData : ITimerData
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "No text provided.";

        [JsonPropertyName("snoozed")]
        public bool Snoozed { get; set; } = false;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
