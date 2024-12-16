using InertiaCore;
using Microsoft.AspNetCore.Http.Features;
using ModCore.Common.Cache;
using ModCore.Services.Web.Attributes;

namespace ModCore.Services.Web.Middleware
{
    public class AttributeMiddleware
    {
        private readonly RequestDelegate _next;
        public AttributeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var attributes = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetOrderedMetadata<RouteMiddlewareAttribute>();
            if (attributes == null || !attributes.Any())
            {
                await _next(context);
                return;
            }

            var next = _next;
            foreach (var attribute in attributes.Reverse())
            {
                var currentNext = next;
                next = async ctx => await attribute.InvokeAsync(ctx, currentNext);
            }

            await next(context);
        }
    }
}
