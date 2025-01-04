using InertiaCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Entities;
using ModCore.Services.Web.Gates;
using ModCore.Services.Web.Middleware;
using ModCore.Services.Web.RequestBodies;
using ModCore.Services.Web.Services;
using ModCore.Services.Web.Validators;
using Npgsql.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Web.Controllers
{
    [Route("dashboard")]
    [ApiController]
    [RouteMiddleware(typeof(RequireAuthentication))]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return Inertia.Render("Dashboard/Dashboard");
        }

        [HttpGet("servers/{server_id}/datadump")]
        public async Task<IActionResult> ServerConfigDownload([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database, [FromServices] CacheService cacheService)
        {
            var userId = ulong.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var gateResponse = await this.GateAsync(new ServerPermissionGate(server_id, Permissions.Administrator));
            if (gateResponse != null)
                return gateResponse;

            var servers = await this.GetCurrentUserGuilds();

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);
            if (dbServer == null)
                return NotFound();

            var dump = GuildDump.Create(server_id, database);

            var serializerOptions = JsonSerializerOptionsFactory.GetOptions();

            return File(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dump, serializerOptions)), "application/json", $"{server_id}.json");
        }

        [HttpGet("servers/{server_id}")]
        public async Task<IActionResult> ServerOverview([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database, [FromServices] CacheService cacheService)
        {
            var userId = ulong.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var gateResponse = await this.GateAsync(new ServerPermissionGate(server_id, Permissions.Administrator));
            if (gateResponse != null)
                return gateResponse;

            var servers = await this.GetCurrentUserGuilds();

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);
            if (dbServer == null)
                return NotFound();

            var serializerOptions = JsonSerializerOptionsFactory.GetOptions();

            var userServer = servers.Where(x => x.Id == server_id).FirstOrDefault();

            return Inertia.Render("Dashboard/Servers/Manage", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions)
            });
        }


        [HttpGet("todo")]
        public async Task<IActionResult> TodoAsync([FromServices] DatabaseContext database)
        {
            return Inertia.Render("Dashboard/Todo");
        }
    }
}
