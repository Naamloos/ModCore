using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Gates;
using ModCore.Services.Web.Middleware;
using ModCore.Services.Web.RequestBodies;
using ModCore.Services.Web.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Database.Entities;
using ModCore.Common.Utils;

namespace ModCore.Services.Web.Controllers
{
    [Route("dashboard/servers/{server_id}/leveling")]
    [RouteMiddleware(typeof(RequireAuthentication))]
    [ApiController]
    public class LevelingSettingsController : ControllerBase
    {
        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet]
        public async Task<IActionResult> LevelingSettings([FromRoute] ulong server_id, [FromServices] DatabaseContext database)
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

            var channels = await this.GetGuildChannelsAsync(server_id);

            var settings = database.LevelSettings.FirstOrDefault(x => x.GuildId == server_id);

            return Inertia.Render("Dashboard/Servers/Leveling/Configure", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions),
                Channels = JsonSerializer.SerializeToDocument(channels, options: serializerOptions),
                Settings = JsonSerializer.SerializeToDocument(settings, options: serializerOptions)
            });
        }

        // Add a post method here to update the leveling settings
    }
}
