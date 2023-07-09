using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public class GuildCreate : Guild, IPublishable
    {
        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonPropertyName("large")]
        public bool IsLarge { get; set; }

        [JsonPropertyName("unavailable")]
        public bool Unavailable { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        [JsonPropertyName("voice_states")]
        public JsonObject[] VoiceStates { get; set; }

        [JsonPropertyName("members")]
        public JsonObject[] Members { get; set; }

        [JsonPropertyName("channels")]
        public JsonObject[] Channels { get; set; }

        [JsonPropertyName("threads")]
        public JsonObject[] Threads { get; set; }

        [JsonPropertyName("presences")]
        public JsonObject[] Presences { get; set; }

        [JsonPropertyName("stage_instances")]
        public JsonObject[] StageInstances { get; set; }

        [JsonPropertyName("guild_scheduled_events")]
        public JsonObject[] GuildScheduledEvents { get; set; }
    }
}
