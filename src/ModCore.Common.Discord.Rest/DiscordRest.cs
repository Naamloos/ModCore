using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    public class DiscordRest
    {
        private DiscordRestConfiguration Configuration;
        private RateLimitedRest RatelimitedRest;
        private JsonSerializerOptions JsonSerializerOptions;

        public DiscordRest(Action<DiscordRestConfiguration> configure, IServiceProvider services)
        {
            Configuration = new DiscordRestConfiguration();
            configure(Configuration);
            JsonSerializerOptions = new JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new SnowflakeJsonSerializer());
            RatelimitedRest = new RateLimitedRest(Configuration, JsonSerializerOptions);
        }

        public Task<RestResponse<User>> GetCurrentUserAsync()
        {
            string route = "users/@me";
            string url = "users/@me";
            return makeRequestAsync<User>(HttpMethod.Get, url, route);
        }

        public Task<RestResponse<User>> GetUserAsync(Snowflake userId)
        {
            string route = "users/:user_id";
            string url = $"users/{userId}";
            return makeRequestAsync<User>(HttpMethod.Get, url, route);
        }

        private async Task<RestResponse<T>> makeRequestAsync<T>(HttpMethod method, string url, string route, object? body = null)
        {
            HttpResponseMessage response = await RatelimitedRest.RequestAsync(HttpMethod.Get, route, url);
            T? deserializedResponse = default(T);
            if (response.IsSuccessStatusCode)
            {
                deserializedResponse = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions);
            }

            return new RestResponse<T>(deserializedResponse, response);
        }
    }
}
