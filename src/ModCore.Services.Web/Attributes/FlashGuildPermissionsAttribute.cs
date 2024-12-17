using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Web.Attributes
{
    public class FlashGuildPermissionsAttribute : Attribute
    {
        private string _serverIdParameter;

        public FlashGuildPermissionsAttribute(string serverIdParameter)
        {
            _serverIdParameter = serverIdParameter;
        }

        public async Task<Permissions> GetPermissionsAsync(HttpContext httpContext)
        {
            if(!ulong.TryParse((string)httpContext.GetRouteValue(_serverIdParameter), out ulong serverId))
            {
                return Permissions.None;
            }
            var userId = ulong.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var restClient = httpContext.RequestServices.GetRequiredService<DiscordRest>();
            var cacheHandler = httpContext.RequestServices.GetRequiredService<CacheService>();

            var guild = await cacheHandler.GetFromCacheOrRest(serverId, (rest, id) => rest.GetGuildAsync(id));
            if (!guild.Success)
                return Permissions.None;
            // get member
            var member = await cacheHandler.GetFromCacheOrRest((serverId, userId), (rest, compositeId)
                => rest.GetGuildMemberAsync(compositeId.serverId, compositeId.userId));

            if (!member.Success)
                return Permissions.None;

            if (guild.Value!.OwnerId == member.Value!.User.Value.Id)
                return Permissions.All;

            // check permissions from member's roles
            var roles = guild.Value.Roles.Where(x => member.Value.Roles.Contains(x.Id));
            var permissions = roles.Aggregate(Permissions.None, (current, role) => current | role.Permissions);

            return permissions;
        }
    }
}
