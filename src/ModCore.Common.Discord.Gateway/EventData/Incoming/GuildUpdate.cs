using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record GuildUpdate : Guild, IPublishable { }
}
