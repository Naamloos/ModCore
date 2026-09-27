using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record InteractionMessageData : InteractionResponseData
    {
        [JsonPropertyName("tts")]
        public Optional<bool> Tts { get; set; }

        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<Attachment[]> Attachments { get; set; }

        [JsonPropertyName("poll")]
        public Optional<PollCreateRequest> Poll { get; set; }
    }
}
