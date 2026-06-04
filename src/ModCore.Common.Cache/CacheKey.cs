using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    public static class CacheKey
    {
        public static string Channels(ulong guildId) => $"channels:{guildId}";

        public static string Roles(ulong guildId) => $"roles:{guildId}";

        public static string Emojis(ulong guildId) => $"emojis:{guildId}";
    }
}
