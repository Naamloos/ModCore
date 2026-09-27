using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ModCore.Common.Language
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddModCoreLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "";
            });

            services.AddSingleton<IModCoreLocalizerFactory, ModCoreLocalizerFactory>();

            return services;
        }
    }
}
