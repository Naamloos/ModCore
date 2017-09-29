using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace ModCore.Entities
{
    public class Settings
    {
        [JsonProperty("token")]
        internal string Token = "Your token";

        [JsonProperty("color")] 
        private string _color = "";
        
        [JsonIgnore]
        public DiscordColor Color => new DiscordColor(_color);

        [JsonProperty("prefix")]
        public string Prefix = "+";

        [JsonProperty("muterole")]
        public ulong MuteRoleId;

        [JsonProperty("blockinvites")]
        public bool BlockInvites = true;

        [JsonProperty("shardcount")]
        public int ShardCount = 1;
    }
}
