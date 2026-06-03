using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            // L1
            services.AddMemoryCache();
#if DEBUG
            // L2 (debug)
            services.AddDistributedMemoryCache();
#else
            // L2 (release)
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config.GetRequiredSection("redis_connection_string").Value!;
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddDistributedRedisCache(setup =>
            {
                // get the configuration from the service collection
                var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                setup.InstanceName = "ModCore";
                setup.Configuration = config.GetRequiredSection("redis_connection_string").Value!;
            });
#endif
            services.AddSingleton<CacheService>();
            services.AddSingleton<IPubSubService, PubSubService>(services =>
            {
                var pubsub = new PubSubService(services.GetService<IConnectionMultiplexer>(), services);

                pubsub.SubscribeAsync<string>("cache_invalidation", (message, serviceProvider) =>
                {
                    var cacheService = serviceProvider.GetRequiredService<CacheService>();
                    cacheService.InvalidateL1(message);
                }).GetAwaiter().GetResult(); // whatever

                return pubsub;
            });
            return services;
        }
    }
}
