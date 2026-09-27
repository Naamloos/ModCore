using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record ChannelCreate : Channel, IPublishable { }
}
