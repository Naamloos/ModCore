using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Allows deserializing obsolete string-based disabled commands, but not serialization.
        /// </summary>
        [JsonProperty("disabledcommands")]
        [Obsolete("Use " + nameof(DisabledCommands) + " instead!")]
        public HashSet<string> LegacyDisabledCommands
        {
            set
            {
                if (DisabledCommands == null)
                {
                    DisabledCommands = new HashSet<short>();
                }
                using (var db = Program.ModCore.CreateGlobalContext())
                {
                    foreach (var s in value)
                    {
                        DisabledCommands.Add(db.CommandIds.FirstOrDefault(e => e.Command == s).Id);
                    }
                }
            }
        }
    }
}