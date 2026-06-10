using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub.Payloads
{
    /// <summary>
    /// Lets all other services know that a cache entry is to be invalidated.
    /// This is used to keep the L1 cache layer up-to-date across all services.
    /// </summary>
    public record InvalidateCachePayload
    {
        public string CacheKey { get; init; } = "";
    }
}
