using Newtonsoft.Json;
using DSharpPlus.Entities;

namespace ModCore.Entities
{
    public class Settings
    {
        [JsonProperty("token")]
        internal string Token = "Your token";

        [JsonProperty("color")]
        string _color = "";
        [JsonIgnore]
        public DiscordColor Color => new DiscordColor(_color);

        [JsonProperty("prefix")]
        public string Prefix = "+";

        [JsonProperty("muterole")]
        public ulong MuteRoleId = 0;

        [JsonProperty("blockinvites")]
        public bool BlockInvites = true;
    }
}
