using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Messages
{
    public class CreateMessage
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<string> Nonce { get; set; }

        [JsonPropertyName("tts")]
        public Optional<bool> IsTTs { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("message_reference")]
        public Optional<MessageReference> MessageReference { get; set; }

        [JsonPropertyName("components")]
        public Optional<MessageComponent[]> Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public Optional<Snowflake[]> StickerIds { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<Attachment> Attachments { get; set; }

        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }
    }
}
