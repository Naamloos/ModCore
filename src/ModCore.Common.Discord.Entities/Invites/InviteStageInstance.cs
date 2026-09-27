using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Invites
{
    public record InviteStageInstance
    {
        [JsonPropertyName("members")]
        public Member[] Members { get; set; } = default!;

        [JsonPropertyName("participant_count")]
        public int ParticipantCount { get; set; }

        [JsonPropertyName("speaker_count")]
        public int SpeakerCount { get; set; }

        [JsonPropertyName("topic")]
        public string Topic { get; set; } = default!;
    }
}
