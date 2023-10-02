using Microsoft.Extensions.DependencyInjection;

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
