using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache.Events;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Cache
{
    public class CacheService
    {
        const int L1_EXPIRATION = 15;
        const int L2_EXPIRATION = 60;

        private IDistributedCache _l2;
        private IMemoryCache _l1;
        private JsonSerializerOptions _serializerOptions;
        private IPubSubService _pubSub { get; set; }
        private ILogger<CacheService> _logger { get; set; }

        public CacheService(IMemoryCache l1, IDistributedCache l2, IPubSubService pubSub, ILogger<CacheService> logger)
        {
            this._l1 = l1;
            this._l2 = l2;
            this._serializerOptions = JsonSerializerOptionsFactory.GetOptions();
            this._pubSub = pubSub;
            this._logger = logger;
        }

        /// <summary>
        /// Tries to get an item from cache. Checks L1 (in-memory) first, then L2 (distributed cache). If found in L2, promotes to L1 for faster access next time.
        /// </summary>
        /// <typeparam name="T">Type of the item to retrieve.</typeparam>
        /// <typeparam name="TKey">Type of the item's ID</typeparam>
        /// <param name="Id">The actual ID of the item to retrieve</param>
        /// <returns>A Cache response that contains item, whether cache was succesful and what cache layer was hit to get this item. The item will be null when success is false.</returns>
        public CacheResponse<T> TryGet<T, TKey>(TKey Id)
        {
            var cacheKey = getCacheKey<T, TKey>(Id);

            var item = default(T);
            var success = false;
            string? resultString = null;
            var layer = CacheLayer.None;

            // Stage 1: L1 Cache (memory cache)
            if (!_l1.TryGetValue<string>(cacheKey, out resultString))
            {
                resultString = _l2.GetString(cacheKey);

                if (resultString != null)
                {
                    // found in layer 2
                    layer = CacheLayer.L2;
                    // Item found in layer 2, populate layer 1 for faster access
                    _l1.Set(cacheKey, resultString, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(L1_EXPIRATION)
                    });
                }
            }
            else
            {
                // found in layer 1
                layer = CacheLayer.L1;
            }

            if (!string.IsNullOrEmpty(resultString))
            {
                try
                {
                    item = JsonSerializer.Deserialize<T>(resultString, _serializerOptions);
                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                    layer = CacheLayer.None;
                    item = default;
                }
            }

            this._logger.LogDebug($"Cache {(success ? "hit" : "miss")} - {typeof(T).Name} with key {cacheKey} on {typeof(CacheLayer).GetEnumName(layer)}");

            return new CacheResponse<T>(success, item, layer);
        }

        public async Task UpdateAsync<T, TKey>(TKey Id, T newItem)
        {
            var cacheKey = getCacheKey<T, TKey>(Id);
            var oldItem = this.TryGet<T, TKey>(Id);

            if(!oldItem.Success)
            {
                await this.storeInCache(cacheKey, JsonSerializer.Serialize(newItem, _serializerOptions));
                this._logger.LogDebug($"Cache update - {typeof(T).Name} with key {cacheKey} (new entity)");
                return;
            }

            var updatedItem = this.mergeObjects(oldItem.Value!, newItem);
            var serializedUpdatedItem = JsonSerializer.Serialize(updatedItem, _serializerOptions);
            this._logger.LogDebug($"Cache update - {typeof(T).Name} with key {cacheKey} (merged)");

            await this.storeInCache(cacheKey, serializedUpdatedItem);
        }

        private async Task storeInCache(string key, string json)
        {
            _l1.Set<string>(key, json, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(L1_EXPIRATION)
            });
            _l2.SetString(key, json, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(L2_EXPIRATION)
            });

            await this._pubSub.PublishAsync<InvalidateCacheKey>("cache", new InvalidateCacheKey(key));
        }

        public void InvalidateL1(string key)
        {
            _l1.Remove(key);

            this._logger.LogDebug($"L1 invalidation: {key}");
        }

        public CacheResponse<MessageHistory> GetMessageFromCache(Snowflake guildId, Snowflake channelId, Snowflake messageId)
        {
            return this.TryGet<MessageHistory, string>($"{guildId}_{channelId}_{messageId}");
        }

        public async Task<MessageHistory?> UpdateCachedMessage(Snowflake guildId, Snowflake channelId, Snowflake messageId, Message? message,
            MessageChangeType changeType)
        {
            MessageHistory? history = null;

            var messageHistoryId = $"{guildId}_{channelId}_{messageId}";

            var newChange = new MessageState()
            {
                ChangeTimestamp = DateTime.UtcNow,
                ChangeType = changeType,
                State = message != default ? message : Optional<Message>.None,
            };

            var oldItem = this.TryGet<MessageHistory, string>(messageHistoryId);

            if (!oldItem.Success)
            {
                history = new MessageHistory()
                {
                    Id = messageId,
                    History = new() { newChange }
                };

                await this.UpdateAsync<MessageHistory, string>(messageHistoryId, history);

                return history;
            }

            history = oldItem.Value!;
            history.History.Add(newChange);
            await this.UpdateAsync<MessageHistory, string>(messageHistoryId, history);

            return history;
        }

        private string getCacheKey<T, TKey>(TKey Id)
        {
            var typeName = typeof(T).Name.Replace(":", "\\:"); // Escape colons in type name
            return $"{typeName}:{Id!.ToString()!.Replace(":", "\\:")}"; // Escape colons in Id
        }

        /// <summary>
        /// Merges a partial update into an existing object.
        /// </summary>
        private T mergeObjects<T>(T existing, T update)
        {
            var existingNode = JsonSerializer.SerializeToNode(existing, _serializerOptions);
            var updateNode = JsonSerializer.SerializeToNode(update, _serializerOptions);

            if (existingNode is JsonObject existingObj && updateNode is JsonObject updateObj)
            {
                mergeNodes(existingObj, updateObj);
                return existingObj.Deserialize<T>(_serializerOptions)!;
            }

            return update;
        }

        /// <summary>
        /// Recursively merges updateNode into existingNode.
        /// </summary>
        private void mergeNodes(JsonObject existing, JsonObject update)
        {
            foreach (var property in update)
            {
                var key = property.Key;
                var newValue = property.Value;

                // 1. Skip nulls or "empty" optional markers if your API defines them as such
                if (newValue == null) continue;

                // 2. If property exists in both as an Object, recurse
                if (existing.TryGetPropertyValue(key, out var existingValue) &&
                    existingValue is JsonObject existingChild &&
                    newValue is JsonObject updateChild)
                {
                    mergeNodes(existingChild, updateChild);
                }
                else
                {
                    // 3. Otherwise, overwrite the existing property with the update
                    // We use DeepClone to ensure no shared references between cache and new data
                    existing[key] = newValue.DeepClone();
                }
            }
        }
    }
}