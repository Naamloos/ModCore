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
            Inertia.Share("appdescription", "A powerful moderating bot written on top of DSharpPlus.");
            Inertia.Share("dotnetversion", Environment.Version);
            await _next(context);
        }
    }
}
