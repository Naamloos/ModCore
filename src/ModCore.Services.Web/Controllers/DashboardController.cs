using InertiaCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Entities;
using ModCore.Services.Web.Gates;
using ModCore.Services.Web.Middleware;
using ModCore.Services.Web.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Web.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return Inertia.Render("Dashboard/Index");
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("servers")]
        public async Task<IActionResult> ServerList([FromServices] UserDiscordRest userDiscord, [FromServices] DatabaseContext database)
        {
            var restClient = await userDiscord.GetDiscordRestAsync();

            if (restClient == null)
            {
                return Redirect("/login");
            }

            // TODO cache, and only list servers that user is admin in
            var servers = await restClient.GetCurrentUserGuilds();

            IEnumerable<Guild> serverList = servers.Value.OrderBy(x => x.Name);
            var serverIds = serverList.Select(x => x.Id.Value).ToArray();
            var dbServerIds = database.Guilds.Where(x => serverIds.Contains(x.GuildId)).Select(x => x.GuildId).ToArray();
            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            var serverListSerialized = serverList.Where(x => dbServerIds.Contains(x.Id.Value))
                .Select(x => JsonSerializer.SerializeToDocument(x, options: serializerOptions))
                .ToList();

            return Inertia.Render("Dashboard/Servers/Index", new
            {
                Servers = serverListSerialized
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("servers/{server_id}/datadump")]
        [FlashGuildPermissions(nameof(server_id))]
        public async Task<IActionResult> ServerConfigDownload([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database)
        {
            var userId = ulong.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var gateResponse = await new ServerPermissionGate(server_id, userId, Permissions.Administrator)
                .CheckAsync(HttpContext);
            if (gateResponse != null)
            {
                return gateResponse;
            }

            var userRestClient = await userDiscord.GetDiscordRestAsync();
            if (userRestClient == null)
            {
                return Redirect("/login");
            }

            var servers = await userRestClient.GetCurrentUserGuilds();
            // check if the server is in the user's guild list
            if (!servers.Success || !servers.Value.Any(x => x.Id == server_id))
            {
                return Redirect("/dashboard");
            }

            var server = await restClient.GetGuildAsync(server_id, true);
            if(!server.Success)
            {
                return NotFound();
            }

            var dump = GuildDump.Create(server_id, database);

            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            return File(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dump, serializerOptions)), "application/json", $"{server_id}.json");
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("servers/{server_id}")]
        [FlashGuildPermissions(nameof(server_id))]
        public async Task<IActionResult> ServerOverview([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database)
        {
            var userId = ulong.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value);

            var gateResponse = await new ServerPermissionGate(server_id, userId, Permissions.Administrator)
                .CheckAsync(HttpContext);
            if (gateResponse != null)
            {
                return gateResponse;
            }

            var userRestClient = await userDiscord.GetDiscordRestAsync();

            if (userRestClient == null)
            {
                return Redirect("/login");
            }

            var servers = await userRestClient.GetCurrentUserGuilds();
            // check if the server is in the user's guild list
            if (!servers.Success || !servers.Value.Any(x => x.Id == server_id))
            {
                return Redirect("/dashboard");
            }

            var server = await restClient.GetGuildAsync(server_id, true);
            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);

            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            return Inertia.Render("Dashboard/Servers/Manage", new
            {
                Server = server.Success ? JsonSerializer.SerializeToDocument(server.Value, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions)
            });
        }
    }
}
