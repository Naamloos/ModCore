using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ModCore.Common.Xaml
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDiscordXaml(
            this IServiceCollection services,
            Assembly assembly
        )
        {
            services.AddSingleton(new DiscordXaml(assembly));
            return services;
        }
    }
}
