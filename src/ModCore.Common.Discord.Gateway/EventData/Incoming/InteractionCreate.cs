using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record InteractionCreate : Interaction, IPublishable
    {

    }
}
