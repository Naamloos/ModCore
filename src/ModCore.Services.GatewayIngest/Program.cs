using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Configuration;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Rest;
using ModCore.Common.PubSub;
using ModCore.Common.Utils;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.GatewayIngest
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

            #if DEBUG
            if (!ConfigurationHelper.CreateEnvFileIfNotExists())
            {
                Console.WriteLine("Created a new env file as it was not found yet. Please fill it out and restart the application.");
                return;
            }
            #endif

            var jsonOptions = JsonSerializerOptionsFactory.GetOptions();

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(options =>
                {
                    options.ClearProviders()
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
                    services.AddDiscordGateway(config =>
                    {
                        config.Intents = Intents.AllUnprivileged | Intents.MessageContents;
                        config.SubscribeEvents(Assembly.GetExecutingAssembly());

                        config.Activity = new Activity()
                        {
                            State = $"the fort nites",
                            Type = ActivityType.Competing,
                        };
                    });
                    services.AddLogging();
                    services.AddSingleton(jsonOptions);
                    services.AddModcoreCacheService();
                    services.AddModCorePubSub();
                })
                .Build();

            host.Run();
        }
    }
}
