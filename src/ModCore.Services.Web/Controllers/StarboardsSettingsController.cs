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
    [Route("dashboard/servers/{server_id}/starboards")]
    [RouteMiddleware(typeof(RequireAuthentication))]
    [ApiController]
    public class StarboardsSettingsController : ControllerBase
    {
        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet]
        public async Task<IActionResult> StarboardsSettings([FromRoute] ulong server_id, [FromServices] DatabaseContext database)
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

            var starboards = database.Starboards.Where(x => x.GuildId == server_id).ToList();

            var emojis = await this.GetGuildEmojisAsync(server_id);

            return Inertia.Render("Dashboard/Servers/Starboards/Configure", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions),
                Channels = JsonSerializer.SerializeToDocument(channels, options: serializerOptions),
                Starboards = JsonSerializer.SerializeToDocument(starboards, options: serializerOptions),
                Emojis = JsonSerializer.SerializeToDocument(emojis, options: serializerOptions)
            });
        }

        // Add a post method here to update the starboards settings
    }
}
