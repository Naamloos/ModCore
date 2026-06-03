
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Web.Gates
{
    public class ServerPermissionGate : IGate
    {
        private readonly ulong _serverId;
        private readonly Permissions _permissions;

        public ServerPermissionGate(ulong serverId, Permissions permissions)
        {
            _serverId = serverId;
            _permissions = permissions;
        }

        public async Task<IActionResult?> CheckAsync(HttpContext httpContext)
        {
            var userId = ulong.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);
            var restClient = httpContext.RequestServices.GetRequiredService<DiscordRest>();
            var cacheHandler = httpContext.RequestServices.GetRequiredService<CacheService>();

            Guild? guild = null;

            var cacheResult = cacheHandler.TryGet<Guild, ulong>(_serverId);
            if (cacheResult.Success)
                guild = cacheResult.Value;
            else
            {
                var restResult = await restClient.GetGuildAsync(_serverId);
                if (restResult.Success)
                    guild = restResult.Value;
            }

            if (guild == null)
                return new UnauthorizedResult();

            // get member
            var member = await restClient.GetGuildMemberAsync(_serverId, userId);

            if (!member.Success)
                return new UnauthorizedResult();

            if (guild!.OwnerId == member.Value!.User.Value.Id)
                return null;

            // check permissions from member's roles
            var roles = guild.Roles.Where(x => member.Value.Roles.Contains(x.Id));
            var permissions = roles.Aggregate(Permissions.None, (current, role) => current | role.Permissions);

            if ((permissions & _permissions) == _permissions)
                return null;

            return new UnauthorizedResult();
        }
    }
}
