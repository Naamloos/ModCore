using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Serializer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Rest
{
    public class DiscordRest
    {
        private DiscordRestConfiguration Configuration;
        private RateLimitedRest RatelimitedRest;
        private JsonSerializerOptions JsonSerializerOptions;
        private ILogger _logger;

        public DiscordRest(Action<DiscordRestConfiguration> configure, IServiceProvider services)
        {
            Configuration = new DiscordRestConfiguration();
            configure(Configuration);
            JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new OptionalJsonSerializerFactory() }
            };

            var hostConfig = services.GetRequiredService<IConfiguration>();

            RatelimitedRest = new RateLimitedRest(Configuration, hostConfig.GetRequiredSection("discord_token").Value, JsonSerializerOptions);

            _logger = services.GetService<ILogger<DiscordRest>>();
        }

        public ValueTask<RestResponse<User>> GetCurrentUserAsync()
        {
            string route = "users/@me";
            string url = "users/@me";
            return makeRequestAsync<User>(HttpMethod.Get, url, route);
        }

        public ValueTask<RestResponse<User>> GetUserAsync(Snowflake userId)
        {
            string route = "users/:user_id";
            string url = $"users/{userId}";
            return makeRequestAsync<User>(HttpMethod.Get, url, route);
        }

        public ValueTask<RestResponse<Application>> GetApplicationAsync(Snowflake applicationId) 
        {
            string route = "applications/:application_id";
            string url = $"applications/{applicationId}";
            return makeRequestAsync<Application>(HttpMethod.Get, url, route);
        }

        // TODO this sucks, properly implement. this is jsut here for "ayo it work" atm.
        public ValueTask<RestResponse<Message>> CreateMessageAsync(Snowflake channelId, CreateMessage content)
        {
            string route = "channels/:channel_id/messages";
            string url = $"channels/{channelId}/messages";
            return makeRequestAsync<Message>(HttpMethod.Post, url, route, content);
        }

        public ValueTask<RestResponse<ApplicationCommand[]>> BulkOverwriteGlobalApplicationCommandsAsync(Snowflake applicationId, params ApplicationCommand[] commands)
        {
            string route = "applications/:application_id/commands";
            string url = $"applications/{applicationId}/commands";
            return makeRequestAsync<ApplicationCommand[]>(HttpMethod.Put, url, route, commands);
        }

        public ValueTask<RestResponse<ApplicationCommand[]>> GetGlobalApplicationCommandsAsync(Snowflake applicationId, bool withLocalizations = true)
        {
            string route = "applications/:application_id/commands";
            string url = $"applications/{applicationId}/commands?with_localizations=" + withLocalizations;
            return makeRequestAsync<ApplicationCommand[]>(HttpMethod.Get, url, route);
        }

        public ValueTask<RestResponse<object>> CreateInteractionResponseAsync(Snowflake interactionId, string interationToken, 
            InteractionResponseType type, InteractionResponseData data)
        {
            string route = "interactions/:interaction_id/:interaction_token/callback";
            string url = $"interactions/{interactionId}/{interationToken}/callback";
            return makeRequestAsync<object>(HttpMethod.Post, url, route, new InteractionResponse()
            {
                Type = type, Data = data
            });
        }

        private async ValueTask<RestResponse<T>> makeRequestAsync<T>(HttpMethod method, string url, string route, object? body = null)
        {
            HttpResponseMessage response = await RatelimitedRest.RequestAsync(method, route, url, body);
            T? deserializedResponse = default(T);
            if (response.IsSuccessStatusCode)
            {
                deserializedResponse = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions);
            }
            else
            {
                _logger.LogError(await response.Content.ReadAsStringAsync());
                _logger.LogError(await response.RequestMessage.Content.ReadAsStringAsync());
            }

            return new RestResponse<T>(deserializedResponse, response);
        }
    }
}
