using InertiaCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Cache;
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
            return Inertia.Render("Dashboard/Dashboard");
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("servers/{server_id}/datadump")]
        public async Task<IActionResult> ServerConfigDownload([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database, [FromServices] CacheService cacheService)
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

            var cacheGuilds = cacheService.TryGet<List<CurrentUserGuild>, string>("userGuilds:" + userId);
            var servers = cacheGuilds.Value;
            if (!cacheGuilds.Success)
            {
                var restGuilds = await userRestClient.GetCurrentUserGuilds();
                servers = restGuilds.Value;
                cacheService.Update("userGuilds:" + userId, servers);
            }
            // check if the server is in the user's guild list
            if (servers == default || !servers.Any(x => x.Id == server_id))
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
        public async Task<IActionResult> ServerOverview([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database, [FromServices] CacheService cacheService)
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

            var cacheGuilds = cacheService.TryGet<List<CurrentUserGuild>, string>("userGuilds:" + userId);
            var servers = cacheGuilds.Value;
            if (!cacheGuilds.Success)
            {
                var restGuilds = await userRestClient.GetCurrentUserGuilds(withCounts: true);
                servers = restGuilds.Value;
                cacheService.Update("userGuilds:" + userId, servers);
            }
            // check if the server is in the user's guild list
            if (servers == default || !servers.Any(x => x.Id == server_id))
            {
                return Redirect("/dashboard");
            }

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);

            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var userServer = servers.Where(x => x.Id == server_id).FirstOrDefault();

            return Inertia.Render("Dashboard/Servers/Manage", new
            {
                Server = userServer != default? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions)
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("servers/{server_id}/welcome")]
        public async Task<IActionResult> WelcomeSettings([FromRoute] ulong server_id,
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient,
            [FromServices] DatabaseContext database, [FromServices] CacheService cacheService)
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

            var cacheGuilds = cacheService.TryGet<List<CurrentUserGuild>, string>("userGuilds:" + userId);
            var servers = cacheGuilds.Value;
            if (!cacheGuilds.Success)
            {
                var restGuilds = await userRestClient.GetCurrentUserGuilds(withCounts: true);
                servers = restGuilds.Value;
                cacheService.Update("userGuilds:" + userId, servers);
            }
            // check if the server is in the user's guild list
            if (servers == default || !servers.Any(x => x.Id == server_id))
            {
                return Redirect("/dashboard");
            }

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);

            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var userServer = servers.Where(x => x.Id == server_id).FirstOrDefault();

            return Inertia.Render("Dashboard/Servers/WelcomeSettings", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions)
            });
        }
    }
}
