using Microsoft.AspNetCore.Mvc;

namespace ModCore.Services.Web.Gates
{
    public interface IGate
    {
        public Task<IActionResult?> CheckAsync(HttpContext httpContext);
    }
}
