﻿using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest.Entities;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public class Ready : IPublishable
    {
        [JsonPropertyName("v")]
        public int ApiVersion { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("guilds")]
        public List<JsonObject> Guilds { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("resume_gateway_url")]
        public string ResumeGatewayUrl { get; set; }
        
        [JsonPropertyName("shard")]
        public int[] Shard { get; set; }

        [JsonPropertyName("application")]
        public ReadyApplication Application { get; set; }
    }

    public class ReadyApplication
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("flags")]
        public int Flags { get; set; }
    }
}