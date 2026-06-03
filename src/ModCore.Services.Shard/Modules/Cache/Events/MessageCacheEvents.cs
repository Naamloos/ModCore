using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;

namespace ModCore.Services.Shard.Modules.Cache.Events
{
    public class MessageCacheEvents : ISubscriber<MessageCreate>, ISubscriber<MessageUpdate>, ISubscriber<MessageDelete>, ISubscriber<MessageBulkDelete>
    {
        private readonly CacheService _cache;

        public MessageCacheEvents(CacheService cacheService)
        {
            _cache = cacheService;
        }

        public Gateway Gateway { get; set; }

        public async ValueTask HandleEvent(MessageCreate data)
            => await UpdateMessage(data, data.GuildId.HasValue ? data.GuildId.Value : 0, data.ChannelId, data.Id, MessageChangeType.Initial);

        public async ValueTask HandleEvent(MessageUpdate data)
            => await UpdateMessage(data, data.GuildId.HasValue ? data.GuildId.Value : 0, data.ChannelId, data.Id, MessageChangeType.Update);

        public async ValueTask HandleEvent(MessageDelete data)
            => await UpdateMessage(null, data.GuildId.HasValue ? data.GuildId.Value : 0, data.ChannelId, data.Id, MessageChangeType.Delete);

        public async ValueTask HandleEvent(MessageBulkDelete data)
        {
            foreach (var id in data.Ids)
                await UpdateMessage(null, data.GuildId.HasValue ? data.GuildId.Value : 0, data.ChannelId, id, MessageChangeType.Delete);
        }

        public async ValueTask UpdateMessage(Message? message, Snowflake guildId, Snowflake channelId, Snowflake messageId, MessageChangeType changeType)
        {
            await _cache.UpdateCachedMessage(guildId, channelId, messageId, message, changeType);
        }
    }
}
