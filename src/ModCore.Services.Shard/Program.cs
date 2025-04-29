using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Rest;
using ModCore.Common.InteractionFramework;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;
using ModCore.Common.Discord.Entities.Serializer;
using System.Text.Json.Serialization;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Utils;
using ModCore.Common.SettingsHelper;
using System.Reflection;
using ModCore.Common.Xaml;
using ModCore.Services.Shard.Modules.Timers.Services;
using ModCore.Services.Shard.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using Sentry.Extensions.Logging;

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

            SettingsHelper.EnsureSettingsExist();

            var jsonOptions = JsonSerializerOptionsFactory.GetOptions();

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config
                        #if DEBUG
                        .AddJsonFile("settings.json") // Only add json config when debugging
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
#if DEBUG
                    services.AddDistributedMemoryCache();
#else
                    services.AddDistributedRedisCache(setup =>
                    {
                        // get the configuration from the service collection
                        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                        setup.InstanceName = "ModCore";
                        setup.Configuration = config.GetRequiredSection("redis_connection_string").Value!;
                    });
#endif
                    services.AddModcoreCacheService();
                    services.AddDbContext<DatabaseContext>();
                    services.AddDiscordXaml(Assembly.GetExecutingAssembly());

                    // Helper for scoped and transient services
                    services.AddSingleton(typeof(TransientService<>), typeof(TransientService<>));


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
                .ConfigureLogging(options =>
                {
                    using var provider = options.Services.BuildServiceProvider();
                    using var scope = provider.CreateScope();
                    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    options
                        .ClearProviders()
                        .AddSerilog(logger)
                        .SetMinimumLevel(LogLevel.Debug);
#if !DEBUG
                    options.AddSentry(config.GetRequiredSection("sentry_shard_dsn").Value!);
#endif
                })
                .Build();


            await AdditionalConfiguration(host);

            host.Run();
        }

        private static async Task AdditionalConfiguration(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                var pendingMigrations = await database.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applied pending database migrations: {0}", string.Join(", ", pendingMigrations));
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation("Pending migration: {0}", migration);
                        await database.Database.MigrateAsync(migration);
                        await database.SaveChangesAsync();
                    }
                }
                else
                {
                    logger.LogInformation("No pending database migrations.");

                    logger.LogError(new IndexOutOfRangeException(), "Sentry Test");
                }
            }
        }
    }
}
