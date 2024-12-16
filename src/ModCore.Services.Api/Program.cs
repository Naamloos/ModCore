using InertiaCore;
using InertiaCore.Extensions;
using ModCore.Services.Web.Middleware;
using System.Diagnostics;

namespace ModCore.Common.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddInertia();

            builder.Services.AddViteHelper(options =>
            {
                options.PublicDirectory = "wwwroot";
                options.BuildDirectory = "build";
                options.ManifestFilename = "manifest.json";
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseInertia();

            app.MapControllers();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<InertiaPropsMiddleware>();

            app.Run();
        }
    }
}