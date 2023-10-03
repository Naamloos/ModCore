using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Gateway.Events;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record GuildCreate : Guild, IPublishable
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
        public Member[] Members { get; set; }

        [JsonPropertyName("channels")]
        public Channel[] Channels { get; set; }

        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; }

        [JsonPropertyName("presences")]
        public JsonObject[] Presences { get; set; }

        [JsonPropertyName("stage_instances")]
        public JsonObject[] StageInstances { get; set; }

        [JsonPropertyName("guild_scheduled_events")]
        public JsonObject[] GuildScheduledEvents { get; set; }
    }
}
