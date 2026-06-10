using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRedisMessaging(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(connectionString));

            services.AddSingleton<IRedisPubSub, RedisPubSub>();

            return services;
        }
    }
}
