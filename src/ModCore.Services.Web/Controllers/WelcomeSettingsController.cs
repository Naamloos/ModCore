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

namespace ModCore.Services.Web.Controllers
{
    [Route("dashboard/servers/{server_id}/welcome")]
    [RouteMiddleware(typeof(RequireAuthentication))]
    [ApiController]
    public class WelcomeSettingsController : ControllerBase
    {
        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet]
        public async Task<IActionResult> WelcomeSettings([FromRoute] ulong server_id, [FromServices] DatabaseContext database)
        {
            // User permissions gate
            var gateResponse = await this.GateAsync(new ServerPermissionGate(server_id, Permissions.Administrator));
            if (gateResponse != null)
                return gateResponse;

            var servers = await this.GetCurrentUserGuilds();

            var dbServer = database.Guilds.FirstOrDefault(x => x.GuildId == server_id);
            if (dbServer == null)
                return NotFound();

            var serializerOptions = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var userServer = servers.Where(x => x.Id == server_id).FirstOrDefault();
            return Inertia.Render("Dashboard/Servers/Welcomer/Configure", new
            {
                Server = userServer != default ? JsonSerializer.SerializeToDocument(userServer, options: serializerOptions) : null,
                DatabaseServer = JsonSerializer.SerializeToDocument(dbServer, options: serializerOptions),
                WelcomeSettings = database.WelcomeSettings.FirstOrDefault(x => x.GuildId == server_id) ?? new DatabaseWelcomeSettings()
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpPost]
        public async Task<IActionResult> UpdateWelcomeSettings([FromRoute] ulong server_id,
            [FromServices] DatabaseContext database,
            [FromBody] WelcomeSettingsUpdate update
        )
        {
            // Validate the request body
            var validated = await this.ValidateAsync(update);
            if (!validated.Success)
            {
                return BadRequest(validated.Message);
            }

            // Get current user's guilds
            var servers = await this.GetCurrentUserGuilds();
            // check if the server is in the user's guild list
            if (servers == default || !servers.Any(x => x.Id == server_id))
            {
                return Unauthorized();
            }

            // User permissions gate
            var gateResponse = await this.GateAsync(new ServerPermissionGate(server_id, Permissions.Administrator));
            if (gateResponse != null)
            {
                return gateResponse;
            }

            var dbWelcome = database.WelcomeSettings.FirstOrDefault(x => x.GuildId == server_id);
            if (dbWelcome == null)
                return NotFound();

            dbWelcome.SetData(update.MessagePayload);
            dbWelcome.Enabled = update.Enabled;
            dbWelcome.ChannelId = update.ChannelId;
            database.Update(dbWelcome);
            await database.SaveChangesAsync();
            return Redirect($"/dashboard/servers/{server_id}");
        }
    }
}
