using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ModCore.Services.Web.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            // check if already authenticated
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            return Challenge(new AuthenticationProperties() { RedirectUri = "/" });
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
