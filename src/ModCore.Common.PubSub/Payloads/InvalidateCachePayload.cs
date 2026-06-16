using ModCore.Common.PubSub.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Common.PubSub.Payloads
{
    /// <summary>
    /// Lets all other services know that a cache entry is to be invalidated.
    /// This is used to keep the L1 cache layer up-to-date across all services.
    /// </summary>
    [EventChannel(EventChannels.InvalidateCache)]
    public record InvalidateCachePayload : IPubSubPayload
    {
        [JsonPropertyName("cache_key")]
        public string CacheKey { get; init; } = "";
    }
}
