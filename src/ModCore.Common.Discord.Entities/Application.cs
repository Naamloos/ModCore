using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
{
    public class Application
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        //public string[] RpcOrigins { get; set; }

        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; set; }

        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; set; }

        [JsonPropertyName("terms_of_service_url")]
        public string? TermsOfServiceUrl { get; set; }

        [JsonPropertyName("privacy_policy_url")]
        public string? PrivacyPolicyUrl { get; set; }

        [JsonPropertyName("owner")]
        public User? Owner { get; set; }

        [JsonPropertyName("team")]
        public JsonObject Team { get; set; }

        [JsonPropertyName("cover_image")]
        public string CoverImage { get; set; }

        [JsonPropertyName("flags")]
        public int Flags { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }
}
