using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework
{
    public class InteractionEventHandler : ISubscriber<InteractionCreate>
    {
        public Gateway Gateway { get; set; }
        private readonly InteractionService _interactions;

        public InteractionEventHandler(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public Task HandleEvent(InteractionCreate data) => _interactions.HandleInteractionAsync(Gateway, data);
    }
}
