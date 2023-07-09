using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModCore.Common.Discord.Gateway;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;

namespace ModCore.Services.Shard
{
    internal class Program
    {
        const string TEMP_TOKEN = "snip";

        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(options =>
                {
                    options
                        .ClearProviders()
                        .AddSerilog(logger)
                        .SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices(services =>
                {
                    // TODO if the rest client is added as a service, 
                    // the gateway will use it to decide what it's websocket url should be.
                    services.AddDiscordGateway(config =>
                    {
                        config.Token = TEMP_TOKEN;
                        config.Intents = Intents.AllUnprivileged;
                    });
                    services.AddDiscordRest(config =>
                    {
                        config.Token = TEMP_TOKEN;
                        config.TokenType = "Bot";
                    });
                    services.AddLogging();
                })
                .Build();

            host.Run();
        }
    }
}
