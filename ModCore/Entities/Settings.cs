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

        [JsonProperty("mute_role")]
        public ulong MuteRoleId;

        [JsonProperty("block_invites")]
        public bool BlockInvites = true;

        [JsonProperty("shard_count")]
        public int ShardCount = 1;

        [JsonProperty("message_cache_size")]
        public int MessageCacheSize = 50;
    }
}
