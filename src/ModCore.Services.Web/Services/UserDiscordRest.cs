using Microsoft.AspNetCore.Authentication;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Web.Services
{
    public class UserDiscordRest
    {
        private IHttpContextAccessor _httpContextAccessor;

        public UserDiscordRest(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext;
        }

        /// <summary>
        /// Gets a session-based discord rest client. Returns null if unauthenticated.
        /// </summary>
        /// <returns></returns>
        public async Task<DiscordRest?> GetDiscordRestAsync()
        {
            if(!_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return null;
            }

            var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            return new DiscordRest(config =>
            {
                config.AuthType = "Bearer";
                config.Token = token;
            });
        }
    }
}
