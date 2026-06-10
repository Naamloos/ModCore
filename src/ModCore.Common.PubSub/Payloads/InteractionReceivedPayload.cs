using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub.Payloads
{
    /// <summary>
    /// A Discord interaction was received either from gateway or from webhook.
    /// </summary>
    /// <typeparam name="T">
    /// Set this to the full interaction payload type. 
    /// This clears the PubSub service from having to import the Discord Entities package.
    /// </typeparam>
    public record InteractionReceivedPayload<T>
    {
        public T Interaction { get; init; } = default!;
    }
}
