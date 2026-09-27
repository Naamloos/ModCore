using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class UnfurledMediaItem
    {
        public string Url { get; set; } = String.Empty;

        [JsonPropertyName("proxy_url")]
        public Optional<string> ProxyUrl { get; set; }

        [JsonPropertyName("height")]
        public Optional<int?> Height { get; set; }

        [JsonPropertyName("width")]
        public Optional<int?> Width { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("placeholder_version")]
        public Optional<int> PlaceholderVersion { get; set; }

        [JsonPropertyName("content_type")]
        public Optional<string> ContentType { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("attachment_id")]
        public Optional<Snowflake> AttachmentId { get; set; }
    }
}
