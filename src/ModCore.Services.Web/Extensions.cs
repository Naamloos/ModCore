using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
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
                cacheService.Update("userGuilds:" + userId, servers);
            }

            return servers.ToList();
        }

        public static async Task<IActionResult?> GateAsync(this ControllerBase controller, IGate gate)
        {
            return await gate.CheckAsync(controller.HttpContext);
        }
    }
}
