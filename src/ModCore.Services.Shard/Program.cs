using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Shard.EventHandlers;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;

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

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true
            };

            if(!File.Exists("settings.json"))
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
                    config.AddJsonFile("settings.json")
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices(services =>
                {
                    // TODO if the rest client is added as a service, 
                    // the gateway will use it to decide what it's websocket url should be.
                    services.AddDiscordGateway(config =>
                    {
                        config.Intents = Intents.AllUnprivileged | Intents.MessageContents;
                        config.SubscribeEvents<StartupEvents>();
                        config.SubscribeEvents<MessageEvents>();
                    });
                    // These are the REAL™️ PISSCATSHARP
                    services.AddDiscordRest(config => { });
                    services.AddLogging();
                })
                .Build();

            host.Run();
        }
    }
}
