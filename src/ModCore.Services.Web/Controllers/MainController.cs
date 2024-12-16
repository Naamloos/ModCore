using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Middleware;
using ModCore.Services.Web.Services;

namespace ModCore.Common.Web.Controllers
{
    [ApiController]
    [Route("/")] // "[Controller]"
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Inertia.Render("Index", new
            {
                dotnetVersion = Environment.Version
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Inertia.Render("Dashboard/Index");
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("dashboard/servers")]
        public async Task<IActionResult> Servers([FromServices] UserDiscordRest userDiscord)
        {
            var restClient = await userDiscord.GetDiscordRestAsync();

            if (restClient == null)
            {
                return Redirect("/login");
            }

            // TODO cache, and only list servers that user is admin in
            var servers = await restClient.GetCurrentUserGuilds();

            var serverList = servers.Value.OrderBy(x => x.Name);

            return Inertia.Render("Dashboard/Servers/Index", new
            {
                Servers = serverList
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("dashboard/servers/{server_id}")]
        public async Task<IActionResult> ManageServer([FromRoute(Name = "server_id")] ulong server_id, 
            [FromServices] UserDiscordRest userDiscord, [FromServices] DiscordRest restClient)
        {
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

            return Inertia.Render("Dashboard/Servers/Manage", new
            {
                Server = server.Success? server.Value : null
            });
        }

        [RouteMiddleware(typeof(RequireAuthentication))]
        [HttpGet("currentUser")]
        public async Task<IActionResult> Test([FromServices]UserDiscordRest userDiscord)
        {
            var restClient = await userDiscord.GetDiscordRestAsync();
            if (restClient == null)
            {
                return Redirect("/login");
            }
            return new JsonResult((await restClient.GetCurrentUserAsync()).Value);
        }
    }
}