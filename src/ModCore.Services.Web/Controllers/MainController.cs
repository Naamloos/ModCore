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

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard([FromServices] UserDiscordRest userDiscord)
        {
            var restClient = await userDiscord.GetDiscordRestAsync();

            if (restClient == null)
            {
                return Redirect("/login");
            }

            // TODO cache, and only list servers that user is admin in
            var servers = await restClient.GetCurrentUserGuilds();

            var serverList = servers.Value.OrderBy(x => x.Name);

            return Inertia.Render("Dashboard/ServerList", new
            {
                Servers = serverList
            });
        }

        [HttpGet("currentUser")]
        public async Task<IActionResult> Test([FromServices]UserDiscordRest userDiscord)
        {
            var restClient = await userDiscord.GetDiscordRestAsync();

            if(restClient == null)
            {
                return Redirect("/login");
            }

            return new JsonResult((await restClient.GetCurrentUserAsync()).Value);
        }

        [HttpGet("test")]
        [RouteMiddleware(typeof(RequireAuthentication))]
        public async Task<IActionResult> Test()
        {
            return new ContentResult()
            {
                Content = "Authorized!",
                ContentType = "text/plain",
                StatusCode = 200
            };
        }
    }
}