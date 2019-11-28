using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.CoreApi.Entities
{
    public class ApiWebhookBody
    {
        [JsonProperty("action")]
        public string ActionType;

        [JsonProperty("params")]
        public JObject ActionParams;
    }

    /// <summary>
    /// On ActionType command
    /// </summary>
    public class CommandActionParams
    {
        [JsonProperty("command")]
        public string CommandName = "ping";

        [JsonProperty("arguments")]
        public string Arguments = "";
    }

    public class MessageActionParams
    {
        [JsonProperty("content")]
        public string Content = "";
    }
}
