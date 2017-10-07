using System;
using Newtonsoft.Json;

namespace ModCore.Entities
{
    public partial class GuildSettings
    {
        /// <summary>
        /// Allows deserializing obsolete InviteBlocker setting, but not serialization.
        /// </summary>
        [JsonProperty("invite_blocker")]
        [Obsolete("Use " + nameof(Linkfilter) + " instead!")]
        public GuildLinkfilterSettings InviteBlocker
        {
            // no getter!
            set => Linkfilter = value;
        }
    }
}