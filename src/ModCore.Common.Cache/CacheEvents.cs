﻿using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    /// <summary>
    /// This event handler exists solely to update memory cache entries when receiving gateway events.
    /// </summary>
    public class CacheEvents : ISubscriber<GuildCreate>, ISubscriber<GuildUpdate>
    {
        public Gateway Gateway { get; set; }

        private readonly ILogger _logger;
        private readonly CacheService _cache;

        public CacheEvents(ILogger<CacheEvents> logger, CacheService cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async ValueTask HandleEvent(GuildCreate data)
        {
            _logger.LogInformation("Updated guild cache for {guildname} via GUILD_CREATE", data.Name);
            _cache.Update<Guild>(data.Id, data);
        }

        public async ValueTask HandleEvent(GuildUpdate data)
        {
            _logger.LogInformation("Updated guild cache for {guildname} via GUILD_UPDATE", data.Name);
            _cache.Update<Guild>(data.Id, data);
        }
    }
}
