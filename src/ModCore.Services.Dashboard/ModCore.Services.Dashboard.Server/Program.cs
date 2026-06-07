using ModCore.Common.Discord.Rest;
using ModCore.Common.SettingsHelper;

namespace ModCore.Services.Dashboard.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SettingsHelper.EnsureSettingsExist();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHttpClient();

            builder.Services.AddDiscordRest(config => { });

            builder.Configuration.AddJsonFile("settings.json");
            builder.Configuration.AddEnvironmentVariables();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
