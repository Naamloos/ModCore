using Microsoft.AspNetCore.Mvc;
using ModCore.Services.DiscordProxy.Services;

namespace ModCore.Services.DiscordProxy.Controllers
{
    [ApiController]
    [Route("api")]
    public class DiscordProxyController : ControllerBase
    {
        private readonly DiscordProxyService _discordProxyService;

        public DiscordProxyController(DiscordProxyService discordProxyService)
        {
            _discordProxyService = discordProxyService;
        }

        [HttpGet("{**path}")]
        [HttpPost("{**path}")]
        [HttpPut("{**path}")]
        [HttpPatch("{**path}")]
        [HttpDelete("{**path}")]
        public async Task ProxyAsync([FromRoute] string? path, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(path))
            {
                Response.StatusCode = 404;
                return;
            }

            await _discordProxyService.ProxyAsync(HttpContext, path, cancellationToken);
        }
    }
}
