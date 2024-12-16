using InertiaCore;
using ModCore.Common.Cache;

namespace ModCore.Services.Web.Middleware
{
    public class RequireAuthentication : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context?.User?.Identity?.IsAuthenticated == true)
            {
                // Redirect to /login, add location to user session
                context.Session.SetString("redirect", context.Request.Path);
                context.Response.Redirect("/login");
                return;
            }

            await next(context);
        }
    }
}
