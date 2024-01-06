using Microsoft.Extensions.Caching.Distributed;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Rest;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Cache
{
    public class CacheService
    {
        private IDistributedCache _cache;
        private JsonSerializerOptions _serializerOptions;
        private DiscordRest DiscordRest { get; set; }

        public CacheService(IDistributedCache cache, DiscordRest restClient) 
        {
            this._cache = cache;
            this._serializerOptions = new JsonSerializerOptions();
        }

        public async ValueTask<(bool Success, T Value)> GetFromCacheOrRest<T>(Snowflake Id, Func<DiscordRest, Snowflake, ValueTask<RestResponse<T>>> fallback)
        {
            T returnValue = default(T);
            bool success = TryGet<T>(Id, out returnValue);
            if (!success)
            {
                var fallbackResponse = await fallback(DiscordRest, Id);
                if(fallbackResponse.Success)
                {
                    returnValue = fallbackResponse.Value!;
                    Update<T>(Id, returnValue);
                    success = true;
                }
            }

            return (success, returnValue);
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

        public bool GetMessageFromCache(Snowflake guildId, Snowflake channelId, Snowflake messageId, out MessageHistory? history)
        {
            history = default;

            var cacheKey = $"message_cache :: {guildId} :: {channelId} :: {messageId}";
            var cachedJson = _cache.GetString(cacheKey);
            if(string.IsNullOrEmpty(cachedJson))
            {
                return false;
            }

            history = JsonSerializer.Deserialize<MessageHistory>(cachedJson);
            return true;
        }

        public void UpdateCachedMessage(Snowflake guildId, Snowflake channelId, Snowflake messageId, Message? message, 
            MessageChangeType changeType, out MessageHistory? history)
        {
            var cacheKey = $"message_cache :: {guildId} :: {channelId} :: {messageId}";
            var cachedJson = _cache.GetString(cacheKey);

            var newChange = new MessageState()
            {
                ChangeTimestamp = DateTime.UtcNow,
                ChangeType = changeType,
                State = message != default ? message : Optional<Message>.None,
            };

            if (string.IsNullOrEmpty(cachedJson))
            {
                history = new MessageHistory()
                {
                    Id = messageId,
                    History = new() { newChange }
                };

                _cache.SetString(cacheKey, JsonSerializer.Serialize(history, _serializerOptions), new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(24) // TODO decide whether there's a more sensible value
                });

                history = history;
                return;
            }

            history = JsonSerializer.Deserialize<MessageHistory>(cachedJson);
            history.History.Add(newChange);

            _cache.SetString(cacheKey, JsonSerializer.Serialize(history, _serializerOptions));
            _cache.Refresh(cacheKey);
        }

        private T mergeObjects<T>(T oldValue, T newValue)
        {
            var type = typeof(T);
            var newCacheItem = (T)Activator.CreateInstance(type)!;

            foreach (var property in type.GetProperties())
            {
                object oldPropertyValue = property.GetValue(oldValue);
                object newPropertyValue = property.GetValue(newValue);

                if(property.PropertyType.IsInstanceOfType(typeof(Optional<>)))
                {
                    // if no value is present in new, we keep old.
                    var prop = property.GetValue(newValue);
                    var hasValue = (bool)prop!.GetType().GetMethod("HasValue")!.Invoke(prop, null)!;
                    if (!hasValue)
                    {
                        property.SetValue(oldPropertyValue, null);
                        continue; // Keep the old value.
                    }
                }

                property.SetValue(newCacheItem, newPropertyValue);
            }

            return newCacheItem;
        }
    }
}