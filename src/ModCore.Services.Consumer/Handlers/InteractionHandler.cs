using Microsoft.Extensions.Logging;
using ModCore.Common.PubSub.Payloads;
using ModCore.Services.Consumer.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Handlers
{
    public class InteractionHandler : BaseHandler<InteractionCreatePayload>
    {
        private readonly ILogger _logger;

        public InteractionHandler(ILogger<InteractionHandler> logger, IEnumerable<IApplicationCommand> commands) {
            _logger = logger;
        }
        
        public override async Task HandleAsync(InteractionCreatePayload payload, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling interaction create payload for shard {ShardId}", payload.ShardId);
        }
    }
}
