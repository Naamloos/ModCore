using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Configuration;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Rest;
using ModCore.Common.InteractionFramework;
using ModCore.Common.Language;
using ModCore.Common.Utils;
using ModCore.Common.Views;
using ModCore.Common.Xaml;
using ModCore.Services.Shard.Abstractions;
using ModCore.Services.Shard.Modules.Timers.Services;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Shard
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            #if DEBUG
            if(!ConfigurationHelper.CreateEnvFileIfNotExists())
            {
                Console.WriteLine("Created a new env file as it was not found yet. Please fill it out and restart the application.");
                return;
            }
            #endif

            var jsonOptions = JsonSerializerOptionsFactory.GetOptions();

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(options =>
                {
                    options
                        .ClearProviders()
                        .AddSerilog(logger)
                        .SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureAppConfiguration(config =>
                {
                    config
                        #if DEBUG
                        .AddEnvFile(ConfigurationHelper.GetDefaultEnvPath())
                        #endif
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices(services =>
                {
                    // TODO if the rest client is added as a service, 
                    // the gateway should use it to decide what it's websocket url should be.
                    services.AddDiscordGateway(config =>
                    {
                        config.Intents = Intents.AllUnprivileged | Intents.MessageContents;
                        config.SubscribeEvents(Assembly.GetExecutingAssembly());

                        config.Activity = new Activity()
                        {
                            State = $"BETA. Not ready for general use.", // TODO move this out 
                            Type = 4
                        };
                    });
                    // These are the REAL™️ PISSCATSHARP
                    services.AddDiscordRest(config => { });
                    services.AddInteractionService();
                    services.AddLogging();
                    services.AddSingleton(jsonOptions);
                    services.AddMemoryCache();
#if DEBUG
                    services.AddDistributedMemoryCache();
#else
                    services.AddDistributedRedisCache(setup =>
                    {
                        // get the configuration from the service collection
                        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                        setup.InstanceName = "ModCore";
                        setup.Configuration = config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.RedisConnectionString)).Value!;
                    });
#endif
                    services.AddModcoreCacheService();
                    services.AddDbContext<DatabaseContext>();
                    services.AddDiscordXaml(Assembly.GetExecutingAssembly());
                    services.AddSingleton<I18n>();

                    // Helper for scoped and transient services
                    services.AddSingleton(typeof(TransientService<>), typeof(TransientService<>));

                    var diagnosticListener = new System.Diagnostics.DiagnosticListener("ModCoreRazor");
                    services.AddSingleton(diagnosticListener);
                    services.AddSingleton<System.Diagnostics.DiagnosticSource>(diagnosticListener);
                    services.AddSingleton<IWebHostEnvironment>(new FakeWebHostEnv
                    {
                        ApplicationName = Assembly.GetExecutingAssembly().GetName().Name!,
                        EnvironmentName = "Development",
                        WebRootPath = Path.GetTempPath(),
                        ContentRootPath = AppContext.BaseDirectory
                    });

                    services.AddMvc(); // includes RazorViewEngine, etc.

                    services.AddSingleton<RazorComponentRenderer>();

                    var types = Assembly.GetExecutingAssembly().GetTypes();
                    foreach(var type in types)
                    {
                        if(!type.IsAbstract 
                            && type.IsClass 
                            && type.IsPublic 
                            && type.GetInterfaces().Contains(typeof(IShardService)))
                        {
                            services.AddSingleton(type);
                        }
                    }
                })
                .Build();

            var config = host.Services.GetService<IConfiguration>();

            await ApplyMigrations(host);

            host.Run();
        }

        private static async Task ApplyMigrations(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var pendingMigrations = await database.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applied pending database migrations: {migrations}", string.Join(", ", pendingMigrations));
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("Pending migration: {migration}", migration);
                    await database.Database.MigrateAsync(migration);
                    await database.SaveChangesAsync();
                }
            }
            else
            {
                logger.LogInformation("No pending database migrations.");
            }
        }
    }

    public class FakeWebHostEnv : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "";
        public string ApplicationName { get; set; } = "";
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
