using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Discord.Rest;
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
    }
}