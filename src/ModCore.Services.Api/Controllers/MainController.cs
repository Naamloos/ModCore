using Microsoft.AspNetCore.Mvc;

namespace ModCore.Common.Api.Controllers
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
        public string Get()
        {
            return "ModCore API";
        }
    }
}