using Microsoft.Extensions.Caching.Distributed;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Cache
{
    public class CacheService
    {
        private IDistributedCache _cache;
        private JsonSerializerOptions _serializerOptions;

        public CacheService(IDistributedCache cache) 
        {
            this._cache = cache;
            this._serializerOptions = new JsonSerializerOptions();
        }

        public bool TryGet<T>(Snowflake Id, out T? item)
        {
            var typeName = typeof(T).Name;
            var cacheKey = $"{typeName} :: {Id}";

            item = default(T);

            var cachedJson = _cache.GetString(cacheKey);
            if(string.IsNullOrEmpty(cachedJson))
                return false;

            item = JsonSerializer.Deserialize<T>(cachedJson);
            return true;
        }

        public void Update<T>(Snowflake Id, T newItem)
        {
            var typeName = typeof(T).Name;
            var cacheKey = $"{typeName} :: {Id}";

            var cachedJson = _cache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cachedJson))
            {
                _cache.SetString(cacheKey, JsonSerializer.Serialize<T>(newItem, _serializerOptions));
                return;
            }

            var cachedItem = JsonSerializer.Deserialize<T>(cachedJson)!;
            var newCacheItem = mergeObjects<T>(cachedItem, newItem);

            _cache.SetString(cacheKey, JsonSerializer.Serialize<T>(newCacheItem, _serializerOptions));
        }

        public bool GetMessageFromCache(Snowflake guildId, Snowflake channelId, Snowflake messageId, out Message? message)
        {
            message = default;

            var cacheKey = $"message_cache :: {guildId} :: {channelId} :: {messageId}";
            var cachedJson = _cache.GetString(cacheKey);
            if(string.IsNullOrEmpty(cachedJson))
            {
                return false;
            }

            message = JsonSerializer.Deserialize<Message>(cachedJson);
            return true;
        }

        public void UpdateCachedMessage(Snowflake guildId, Snowflake channelId, Snowflake messageId, Message message)
        {
            var cacheKey = $"message_cache :: {guildId} :: {channelId} :: {messageId}";
            var cachedJson = _cache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cachedJson))
            {
                _cache.SetString(cacheKey, JsonSerializer.Serialize<Message>(message, _serializerOptions));
                return;
            }

            var oldMessage = JsonSerializer.Deserialize<Message>(cachedJson);
            var newMessage = mergeObjects(oldMessage, message);
            _cache.SetString(cacheKey, JsonSerializer.Serialize<Message>(newMessage, _serializerOptions));
        }

        private T mergeObjects<T>(T oldValue, T newValue)
        {
            var newCacheItem = oldValue;

            var type = typeof(T);

            foreach (var property in type.GetProperties())
            {
                object newPropertyValue = property.GetValue(newValue);

                if(property.PropertyType.IsInstanceOfType(typeof(Optional<>)))
                {
                    // if no value is present in new, we don't merge.
                    var prop = property.GetValue(newValue);
                    var hasValue = (bool)prop!.GetType().GetMethod("HasValue")!.Invoke(prop, null)!;
                    if (!hasValue)
                    {
                        continue; // Optional has no value meaning we can ignore this property. Keep the old value.
                    }
                }

                property.SetValue(newCacheItem, newPropertyValue);
            }

            return newCacheItem;
        }
    }
}