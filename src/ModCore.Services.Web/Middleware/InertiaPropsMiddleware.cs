using InertiaCore;

namespace ModCore.Services.Web.Middleware
{
    public class InertiaPropsMiddleware
    {
        private readonly RequestDelegate _next;
        public InertiaPropsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Inertia.Share("apptitle", "ModCore");
            Inertia.Share("appdescription", 
@"ModCore is your assistant for Discord server moderation and management 
through a wide range of hand-crafted features to make your life as a moderator or administrator a breeze!"
            );

            Inertia.Share("authenticated", context.User.Identity.IsAuthenticated);

            Inertia.Share("user", context.User.Identity.IsAuthenticated? new
            {
                username = context.User.Identity.Name,
                avatar = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:url")?.Value
            } : null);

            await _next(context);
        }
    }
}
