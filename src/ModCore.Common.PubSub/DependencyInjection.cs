using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Configuration;
using ModCore.Common.PubSub.Implementation;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddModCorePubSub(
            this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var configValue = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.RedisConnectionString))
                    .Value!;

                // If it contains the protocol prefix, parse it properly
                if (configValue.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(configValue);
                    var options = new ConfigurationOptions
                    {
                        EndPoints = { { uri.Host, uri.Port } },
                        AbortOnConnectFail = false // Highly recommended for Docker setups!
                    };
                    return ConnectionMultiplexer.Connect(options);
                }

                // Fallback for standard string formats (e.g., "localhost:6379")
                return ConnectionMultiplexer.Connect(configValue);
            });

            services.AddSingleton<IPubSub, RedisPubSub>();
            services.AddSingleton<PubSubService>();

            return services;
        }
    }
}
