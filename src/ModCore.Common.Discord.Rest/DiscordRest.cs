using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            var hostConfig = services.GetRequiredService<IConfiguration>();

            RatelimitedRest = new RateLimitedRest(Configuration, hostConfig.GetRequiredSection("discord_token").Value, JsonSerializerOptions);
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

        public Task<RestResponse<Application>> GetApplicationAsync(Snowflake applicationId) 
        {
            string route = "applications/:application_id";
            string url = $"applications/{applicationId}";
            return makeRequestAsync<Application>(HttpMethod.Get, url, route);
        }

        public class msg
        {
            [JsonPropertyName("content")]
            public string content { get; set; }
        }

        // TODO this sucks, properly implement. this is jsut here for "ayo it work" atm.
        public Task<RestResponse<Message>> CreateMessageAsync(Snowflake channelId, string content)
        {
            string route = "channels/:channel_id/messages";
            string url = $"channels/{channelId}/messages";
            var message = new msg() { content = content };
            return makeRequestAsync<Message>(HttpMethod.Post, url, route, message);
        }

        private async Task<RestResponse<T>> makeRequestAsync<T>(HttpMethod method, string url, string route, object? body = null)
        {
            HttpResponseMessage response = await RatelimitedRest.RequestAsync(method, route, url, body);
            T? deserializedResponse = default(T);
            if (response.IsSuccessStatusCode)
            {
                deserializedResponse = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions);
            }

            return new RestResponse<T>(deserializedResponse, response);
        }
    }
}
