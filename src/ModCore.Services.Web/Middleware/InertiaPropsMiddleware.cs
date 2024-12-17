using InertiaCore;
using Microsoft.AspNetCore.Http.Features;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Services.Web.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
            Inertia.Share("user", context.User.Identity.IsAuthenticated? new
            {
                id = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:id")?.Value,
                username = context.User.Identity.Name,
                avatar = context.User.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:url")?.Value
            } : null);

            var cacheService = context.RequestServices.GetRequiredService<CacheService>();
            var configService = context.RequestServices.GetRequiredService<IConfiguration>();
            ulong applicationId = ulong.Parse(configService["discord_client_id"]);
            var app = await cacheService.GetFromCacheOrRest(applicationId, (rest, id) =>
            {
                return rest.GetApplicationAsync(id);
            });

            // workaround for Optional<T> serialization
            var serializedApp = JsonSerializer.SerializeToDocument(app.Value, options: new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            });

            Inertia.Share("application", app.Success ? serializedApp : null);

            var attributes = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetOrderedMetadata<FlashGuildPermissionsAttribute>();
            if (attributes != null && attributes.Any())
            {
                var perms = await attributes[0].GetPermissionsAsync(context);
                // convert enum flags to string array
                var permissionStrings = Enum.GetValues(typeof(Permissions)).Cast<Permissions>()
                    .Where(p => perms.HasFlag(p) && p != Permissions.None && p != Permissions.All).Select(p => p.ToString());
                // PascaleCase to space separated
                var permissionNames = permissionStrings.Select(p => Regex.Replace(p, "(\\B[A-Z])", " $1"));
                Inertia.Share("permissions", permissionNames);
            }

            await _next(context);
        }
    }
}
