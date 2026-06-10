using ModCore.Common.Configuration;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Dashboard.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #if DEBUG
            if (!ConfigurationHelper.CreateEnvFileIfNotExists())
            {
                Console.WriteLine("Created a new env file as it was not found yet. Please fill it out and restart the application.");
                return;
            }
            #endif

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHttpClient();

            builder.Services.AddDiscordRest(config => { });

            #if DEBUG
            builder.Configuration.AddEnvFile(ConfigurationHelper.GetDefaultEnvPath());
            #endif
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
