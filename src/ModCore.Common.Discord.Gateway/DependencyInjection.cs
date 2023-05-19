using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDiscordGateway(this IServiceCollection services, Action<GatewayConfiguration> configure)
        {
            services.AddHostedService(serviceProvider => new Gateway(configure, serviceProvider));
            return services;
        }
    }
}
