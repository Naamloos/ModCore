using InertiaCore;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;

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

            // Ensure user exists in db.
            var db = context.RequestServices.GetRequiredService<DatabaseContext>();
            var uid = context.User.Claims.FirstOrDefault(x => x.Type == "urn:discord:id")?.Value;
            var ulongUid = ulong.Parse(uid);
            if (!db.Users.Any(x => x.UserId == ulongUid))
            {
                // create new db user
                db.Users.Add(new DatabaseUser
                {
                    UserId = ulongUid
                });
                await db.SaveChangesAsync();
            }

            await next(context);
        }
    }
}
