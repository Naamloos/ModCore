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
using ModCore.Services.Shard.EventHandlers;
using ModCore.Common.Database;
using ModCore.Common.Utils;

namespace ModCore.Services.Shard
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var jsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                WriteIndented = true
            };

            if (!File.Exists("settings.json"))
            {
                File.Create("settings.json").Close();
                File.WriteAllText("settings.json", JsonSerializer.Serialize(new Settings(), jsonOptions));
                logger.Information("Settings file not found. Created one. Please fill with required values!");
                return;
            }
            else
            {
                // ensure new config values are written
                var contents = File.ReadAllText("settings.json");
                var settings = JsonSerializer.Deserialize<Settings>(contents);
                File.WriteAllText("settings.json", JsonSerializer.Serialize(settings, jsonOptions));
            }

            jsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new OptionalJsonSerializerFactory() },
                WriteIndented = true
            };

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
                        config.SubscribeEvents<StartupEvents>();
                        config.SubscribeEvents<MessageEvents>();
                        config.SubscribeEvents<SimpleEvalEvent>();

                        // These events live in the cache service.
                        config.SubscribeEvents<CacheEvents>();

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
                    services.AddDistributedMemoryCache();
                    services.AddModcoreCacheService();
                    services.AddDbContext<DatabaseContext>();

                    // Helper for scoped and transient services
                    services.AddSingleton(typeof(TransientService<>), typeof(TransientService<>));

                    // Shard-specific services
                    services.AddSingleton<TimerService>();
                })
                .Build();

            host.Run();
        }
    }
}
