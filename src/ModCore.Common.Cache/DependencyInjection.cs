using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<CacheService>();
            return services;
        }
    }
}
