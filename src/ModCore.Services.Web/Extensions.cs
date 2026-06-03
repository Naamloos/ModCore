using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Web.Gates;
using ModCore.Services.Web.Services;
using ModCore.Services.Web.Validators;

namespace ModCore.Services.Web
{
    public static class Extensions
    {
        public static Task<ValidatorResult<T>> ValidateAsync<T>(this ControllerBase controller, T value)
        {
            // find validator with reflection
            var validator = typeof(IValidator<>).Assembly.DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(IValidator<T>)))
                .FirstOrDefault();
            if (validator == null)
            {
                throw new InvalidOperationException($"No validator found for type {typeof(T).Name}");
            }
            var validatorInstance = (IValidator<T>)Activator.CreateInstance(validator)!;
            return validatorInstance.ValidateAsync(controller.HttpContext, value);
        }

        public static async Task<List<CurrentUserGuild>> GetCurrentUserGuilds(this ControllerBase controller)
        {
            var userId = ulong.Parse(controller.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var cacheService = controller.HttpContext.RequestServices.GetRequiredService<CacheService>();
            var userRestClient = await controller.HttpContext.RequestServices.GetRequiredService<UserDiscordRest>().GetDiscordRestAsync();

            var cacheGuilds = cacheService.TryGet<List<CurrentUserGuild>, string>("userGuilds:" + userId);
            var servers = cacheGuilds.Value;
            if (!cacheGuilds.Success)
            {
                var restGuilds = await userRestClient.GetCurrentUserGuilds(withCounts: true);
                servers = restGuilds.Value;
                cacheService.UpdateAsync("userGuilds:" + userId, servers);
            }

            return servers.ToList();
        }

        public static async Task<IActionResult?> GateAsync(this ControllerBase controller, IGate gate)
        {
            return await gate.CheckAsync(controller.HttpContext);
        }

        public static async Task<List<Channel>> GetGuildChannelsAsync(this ControllerBase controller, ulong server_id)
        {
            var cacheService = controller.HttpContext.RequestServices.GetRequiredService<CacheService>();
            var restClient = controller.HttpContext.RequestServices.GetRequiredService<DiscordRest>();
            List<Channel>? channels = null;

            var cacheResponse = cacheService.TryGet<List<Channel>, string>($"guild_channels:{server_id}");
            if(cacheResponse.Success)
            {
                channels = cacheResponse.Value;
            }
            else
            {
                var restResponse = await restClient.GetGuildChannelsAsync(server_id);
                if (restResponse.Success)
                {
                    channels = restResponse.Value;
                    await cacheService.UpdateAsync($"guild_channels:{server_id}", channels);
                }
            }

            return channels ?? new List<Channel>();
        }

        public static async Task<List<Emoji>> GetGuildEmojisAsync(this ControllerBase controller, ulong server_id)
        {
            var cacheService = controller.HttpContext.RequestServices.GetRequiredService<CacheService>();
            var restClient = controller.HttpContext.RequestServices.GetRequiredService<DiscordRest>();

            List<Emoji>? emojis = null;

            var cacheResponse = cacheService.TryGet<List<Emoji>, string>($"emojis:{server_id}");
            if (cacheResponse.Success)
            {
                emojis = cacheResponse.Value;
            }
            else
            {
                var restResponse = await restClient.GetGuildEmojisAsync(server_id);
                if (restResponse.Success)
                {
                    emojis = restResponse.Value;
                    await cacheService.UpdateAsync($"emojis:{server_id}", emojis);
                }
            }

            return emojis ?? new List<Emoji>();
        }

        public static async Task<Role[]> GetGuildRolesAsync(this ControllerBase controller, ulong server_id)
        {
            var cacheService = controller.HttpContext.RequestServices.GetRequiredService<CacheService>();
            var restClient = controller.HttpContext.RequestServices.GetRequiredService<DiscordRest>();

            List<Role>? roles = null;

            var cacheResponse = cacheService.TryGet<List<Role>, string>($"roles:{server_id}");
            if (cacheResponse.Success)
            {
                roles = cacheResponse.Value;
            }
            else { 
                var restResponse = await restClient.GetGuildRolesAsync(server_id);
                if (restResponse.Success)
                {
                    roles = restResponse.Value;
                    await cacheService.UpdateAsync($"roles:{server_id}", roles);
                }
            }

            return roles.ToArray() ?? new Role[0];
        }
    }
}
