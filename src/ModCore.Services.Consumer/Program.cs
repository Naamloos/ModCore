using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Configuration;
using ModCore.Common.Database;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Language;
using ModCore.Common.PubSub;
using ModCore.Common.Utils;
using ModCore.Services.Consumer.Handlers;
using ModCore.Services.Consumer.Interactions.Framework;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

namespace ModCore.Services.Consumer
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
                    services.AddDiscordRest(config => { });
                    services.AddLogging();
                    services.AddSingleton(jsonOptions);
                    services.AddModcoreCacheService();
                    services.AddDbContext<DatabaseContext>();
                    services.AddModCorePubSub();
                    services.AddStackExchangeRedisCache(setup =>
                    {
                        // get the configuration from the service collection
                        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                        setup.InstanceName = "ModCore";
                        setup.Configuration = config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.RedisConnectionString)).Value!;
                    });

                    // Register every handler as a service
                    var handlerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(BaseHandler)));
                    foreach(var handlerType in handlerTypes)
                    {
                        services.AddSingleton(handlerType);
                    }

                    services.AddModCoreLocalization();

                    var assembly = Assembly.GetExecutingAssembly();
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }

                        if (typeof(BaseApplicationCommand).IsAssignableFrom(type))
                        {
                            services.AddScoped(typeof(IApplicationCommand), type);
                            continue;
                        }

                        if (typeof(BaseApplicationSubcommand).IsAssignableFrom(type))
                        {
                            services.AddScoped(typeof(IApplicationSubcommand), type);
                        }
                    }


                    // Consumer service!
                    services.AddHostedService<ConsumerHostedService>();
                })
                .Build();

            host.Run();
        }
    }
}
