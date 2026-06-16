using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddModcoreCacheService(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var redisConfig = GetRedisConfiguration(config);

                return ConnectionMultiplexer.Connect(redisConfig);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = "ModCore:";
                options.ConfigurationOptions = null;
            });

            services.AddOptions<RedisCacheOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    options.InstanceName = "ModCore:";
                    options.ConfigurationOptions = GetRedisConfiguration(config);
                });

            services.AddSingleton<CacheService>();

            services.AddSingleton<IPubSubService, PubSubService>(sp =>
            {
                var pubsub = new PubSubService(
                    sp.GetRequiredService<IConnectionMultiplexer>(),
                    sp);

                pubsub.SubscribeAsync<string>("cache_invalidation", (message, serviceProvider) =>
                {
                    var cacheService = serviceProvider.GetRequiredService<CacheService>();
                    cacheService.InvalidateL1(message);
                }).GetAwaiter().GetResult();

                return pubsub;
            });

            return services;
        }

        private static ConfigurationOptions GetRedisConfiguration(IConfiguration config)
        {
            var value = config
                .GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.RedisConnectionString))
                .Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Redis connection string is missing.");
            }

            if (value.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(value);

                var options = new ConfigurationOptions
                {
                    AbortOnConnectFail = false
                };

                options.EndPoints.Add(uri.Host, uri.Port);

                if (!string.IsNullOrWhiteSpace(uri.UserInfo))
                {
                    // Supports redis://:password@localhost:6379
                    var parts = uri.UserInfo.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        options.Password = Uri.UnescapeDataString(parts[1]);
                    }
                }

                return options;
            }

            var parsed = ConfigurationOptions.Parse(value);
            parsed.AbortOnConnectFail = false;
            return parsed;
        }
    }
}