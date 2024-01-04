using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInteractionService(this IServiceCollection services)
        {
            services.AddSingleton<InteractionService>();
            return services;
        }
    }
}
