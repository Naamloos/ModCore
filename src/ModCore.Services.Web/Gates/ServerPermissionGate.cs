
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Web.Gates
{
    public class ServerPermissionGate : IGate
    {
        private readonly ulong _serverId;
        private readonly ulong _userId;
        private readonly Permissions _permissions;

        public ServerPermissionGate(ulong serverId, ulong userId, Permissions permissions)
        {
            _serverId = serverId;
            _userId = userId;
            _permissions = permissions;
        }

        public async Task<IActionResult?> CheckAsync(HttpContext httpContext)
        {
            var restClient = httpContext.RequestServices.GetRequiredService<DiscordRest>();
            var cacheHandler = httpContext.RequestServices.GetRequiredService<CacheService>();
            
            var guild = await cacheHandler.GetFromCacheOrRest(_serverId, (rest, id) => rest.GetGuildAsync(id));
            if (!guild.Success)
                return new UnauthorizedResult();
            // get member
            var member = await restClient.GetGuildMemberAsync(_serverId, _userId);

            if (!member.Success)
                return new UnauthorizedResult();

            if(guild.Value!.OwnerId == member.Value!.User.Value.Id)
                return null;

            // check permissions from member's roles
            var roles = guild.Value.Roles.Where(x => member.Value.Roles.Contains(x.Id));
            var permissions = roles.Aggregate(Permissions.None, (current, role) => current | role.Permissions);

            if ((permissions & _permissions) == _permissions)
                return null;

            return new UnauthorizedResult();
        }
    }
}
