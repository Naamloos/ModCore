using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    /// <summary>
    /// This event handler exists solely to update memory cache entries when receiving gateway events.
    /// </summary>
    public class CacheEvents : ISubscriber<GuildCreate>, ISubscriber<GuildUpdate>, 
        ISubscriber<MessageCreate>, ISubscriber<MessageUpdate>, ISubscriber<MessageDelete>, ISubscriber<MessageBulkDelete>
    {
        public Gateway Gateway { get; set; }

        private readonly ILogger _logger;
        private readonly CacheService _cache;

        public CacheEvents(ILogger<CacheEvents> logger, CacheService cache)
        {
            _logger = logger;
            _cache = cache;
        }

        // TODO: member+user+channel+role cache,

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

        // The following 4 methods keep a local message history cache, which is essentially very useful for moderators trying to snipe multiple edits.
        // This cache expires after 24 hours, but I might lower that amount if cache fills up too quick.
        public async ValueTask HandleEvent(MessageCreate data)
        {
            _cache.UpdateCachedMessage(data.GuildId, data.ChannelId, data.Id, data, MessageChangeType.Initial, out _);
        }

        public async ValueTask HandleEvent(MessageUpdate data)
        {
            _cache.UpdateCachedMessage(data.GuildId, data.ChannelId, data.Id, data, MessageChangeType.Update, out _);
        }

        public async ValueTask HandleEvent(MessageDelete data)
        {
            _cache.UpdateCachedMessage(data.GuildId, data.ChannelId, data.Id, null, MessageChangeType.Delete, out _); 
            // Why do I even have an out variable here
        }

        public async ValueTask HandleEvent(MessageBulkDelete data)
        {
            foreach(var id in data.Ids)
            {
                _cache.UpdateCachedMessage(data.GuildId, data.ChannelId, id, null, MessageChangeType.BulkDelete, out _);
            }
        }
    }
}
