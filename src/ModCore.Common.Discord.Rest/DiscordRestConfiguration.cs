using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public record DiscordRestConfiguration
    {
        public string Token { get; set; }
        public string TokenType { get; set; } // TODO enum
    }
}
