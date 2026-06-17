using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Configuration;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Rest;
using ModCore.Common.PubSub;
using ModCore.Common.Utils;
using Quartz;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Services.Jobs
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
                        //.AddJsonFile("settings.json") // Only add json config when debugging
                        .AddEnvFile(ConfigurationHelper.GetDefaultEnvPath())
                        #endif
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices(services =>
                {
                    services.AddQuartz();
                    services.AddQuartzHostedService(opt =>
                    {
                        opt.WaitForJobsToComplete = true;
                        opt.AwaitApplicationStarted = true;
                    });

                    //services.AddDiscordRest(x => { });

                    //services.AddDbContext<DatabaseContext>();
                    //services.AddDistributedMemoryCache();
                    //services.AddModcoreCacheService();

                    services.AddDiscordRest(config => { });
                    services.AddLogging();
                    services.AddSingleton(jsonOptions);
                    services.AddDbContext<DatabaseContext>();

                    services.AddModcoreCacheService();
                    services.AddModCorePubSub();

                    services.AddSingleton<TimerScheduler>();
                    services.AddHostedService<TimerSchedulerHost>();

                    services.AddSingleton(typeof(TransientService<>), typeof(TransientService<>));
                })
                .Build();

            var schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            var mcoreJobBuilder = new ModCoreJobBuilder(scheduler);
            mcoreJobBuilder.BuildAndScheduleJobs();

            await host.RunAsync();
        }
    }
}
