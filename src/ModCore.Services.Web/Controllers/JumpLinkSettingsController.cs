using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Utils;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Gates;
using ModCore.Services.Web.Middleware;
using System.Text.Json;

namespace ModCore.Services.Web.Controllers
{
    [Route("dashboard/servers/{server_id}/jumplink")]
    [RouteMiddleware(typeof(RequireAuthentication))]
    [ApiController]
    public class JumpLinkSettingsController : ControllerBase
    {
        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet]
        public async Task<IActionResult> LoggingSettings([FromRoute] ulong server_id, [FromServices] DatabaseContext database)
        {
            // User permissions gate
            var gateResponse = await this.GateAsync(new ServerPermissionGate(server_id, Permissions.Administrator));
            if (gateResponse != null)
                return gateResponse;

            var servers = await this.GetCurrentUserGuilds();

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);
            if (dbServer == null)
                return NotFound();

            var serializerOptions = JsonSerializerOptionsFactory.GetOptions();

            var userServer = servers.Where(x => x.Id == server_id).FirstOrDefault();

            return Inertia.Render("Dashboard/Servers/JumpLinkEmbed/Configure", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions),
            });
        }
    }
}
