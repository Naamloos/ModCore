namespace ModCore.Services.Web.Attributes
{
    public class RouteMiddlewareAttribute : Attribute
    {
        public Type Middleware { get; set; }
        public RouteMiddlewareAttribute(Type t)
        {
            if(!typeof(IMiddleware).IsAssignableFrom(t))
            {
                throw new ArgumentException("Middleware must implement IMiddleware");
            }
            Middleware = t;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var middleware = Activator.CreateInstance(Middleware) as IMiddleware;
            await middleware.InvokeAsync(context, next);
        }
    }
}
