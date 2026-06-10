using InertiaCore;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using ModCore.Common.Cache;
using ModCore.Common.Configuration;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ModCore.Services.Web.Middleware
{
    public class InertiaPropsMiddleware
    {
        private readonly RequestDelegate _next;
        public InertiaPropsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Inertia.Share("user", context.User.Identity.IsAuthenticated? new
            {
                id = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:id")?.Value,
                username = context.User.Identity.Name,
                avatar = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:url")?.Value
            } : null);

            var cacheService = context.RequestServices.GetRequiredService<CacheService>();
            var configService = context.RequestServices.GetRequiredService<IConfiguration>();
            var discordRest = context.RequestServices.GetRequiredService<DiscordRest>();
            ulong applicationId = ulong.Parse(configService[ConfigurationHelper.GetConfigKeyString(ConfigKey.ClientId)]);

            Application? app = null;

            var cacheResponse = cacheService.TryGet<Application, ulong>(applicationId);
            if(cacheResponse.Success)
            {
                app = cacheResponse.Value;
            }
            else
            {
                var restResponse = await discordRest.GetApplicationAsync(applicationId);
                if(restResponse.Success)
                {
                    app = restResponse.Value;
                    await cacheService.UpdateAsync(applicationId, app);
                }
            }

            if(app == null)
            {
                throw new Exception("Failed to retrieve application information");
            }

            // workaround for Optional<T> serialization
            var serializedApp = JsonSerializer.SerializeToDocument(app, options: JsonSerializerOptionsFactory.GetOptions());

            Inertia.Share("application", serializedApp);

            if(context.User.Identity.IsAuthenticated)
            {
                var databaseContext = context.RequestServices.GetRequiredService<DatabaseContext>();
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:id")?.Value;
                var userRest = await context.RequestServices.GetRequiredService<UserDiscordRest>().GetDiscordRestAsync();
                
                var cacheGuilds = cacheService.TryGet<List<CurrentUserGuild>, string>("userGuilds:" + userId);
                var guilds = cacheGuilds.Value;
                if (!cacheGuilds.Success)
                {
                    var restGuilds = await userRest.GetCurrentUserGuilds();
                    guilds = restGuilds.Value;
                    await cacheService.UpdateAsync("userGuilds:" + userId, guilds);
                }

                var serverIds = guilds.Select(x => x.Id.Value).ToArray();
                var dbServerIds = databaseContext.Guilds.Where(x => serverIds.Contains(x.GuildId)).Select(x => x.GuildId).ToArray();
                var serializerOptions = JsonSerializerOptionsFactory.GetOptions();
                var serverListSerialized = guilds.Where(x => dbServerIds.Contains(x.Id.Value))
                    .Where(x => x.Permissions.HasFlag(Permissions.ManageGuild))
                    .Select(x => JsonSerializer.SerializeToDocument(x, options: serializerOptions))
                    .ToList();

                Inertia.Share("user_guilds", serverListSerialized);
            }

            await _next(context);
        }
    }
}
