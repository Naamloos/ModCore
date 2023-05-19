using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public class Ready
    {
        [JsonPropertyName("v")]
        public int ApiVersion { get; internal set; }

        [JsonPropertyName("user")]
        public JsonObject User { get; internal set; }

        [JsonPropertyName("guilds")]
        public ReadOnlyCollection<JsonObject> Guilds { get; internal set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; internal set; }

        [JsonPropertyName("resume_gateway_url")]
        public string ResumeGatewayUrl { get; internal set; }
        
        [JsonPropertyName("shard")]
        public int[] Shard { get; internal set; }

        [JsonPropertyName("application")]
        public JsonObject Application { get; internal set; }
    }
}
