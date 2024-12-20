using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Web.Attributes;
using ModCore.Services.Web.Gates;
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
            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                // Redirect to /dashboard
                return Redirect("/dashboard");
            }

            return Inertia.Render("Home", new
            {
                dotnetVersion = Environment.Version
            });
        }
    }
}