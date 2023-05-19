using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDiscordRest(this IServiceCollection services, Action<DiscordRestConfiguration> configure)
        {
            services.AddSingleton(serviceProvider => new DiscordRest(configure, serviceProvider));
            return services;
        }
    }
}
