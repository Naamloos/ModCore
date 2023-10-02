using Microsoft.Extensions.DependencyInjection;

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
