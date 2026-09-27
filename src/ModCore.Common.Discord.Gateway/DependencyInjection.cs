using Microsoft.Extensions.DependencyInjection;

namespace ModCore.Common.Discord.Gateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDiscordGateway(
            this IServiceCollection services,
            Action<GatewayConfiguration> configure
        )
        {
            services.AddSingleton(serviceProvider => new Gateway(configure, serviceProvider));
            services.AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<Gateway>()
            );
            return services;
        }

        public static IServiceCollection AddDiscordGateway(
            this IServiceCollection services,
            Action<IServiceProvider, GatewayConfiguration> configure
        )
        {
            services.AddSingleton(serviceProvider => new Gateway(
                configuration => configure(serviceProvider, configuration),
                serviceProvider
            ));
            services.AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<Gateway>()
            );
            return services;
        }
    }
}
