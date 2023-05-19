using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public class DiscordRest
    {
        const short API_VERSION = 10;

        private HttpClient httpClient;
        private DiscordRestConfiguration configuration;

        public DiscordRest(Action<DiscordRestConfiguration> configure, IServiceProvider services)
        {
            configuration = new DiscordRestConfiguration();
            configure(configuration);

            httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"https://discord.com/api/v{API_VERSION}")
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", $"{configuration.TokenType} {configuration.Token}");
        }
    }
}
