using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Entities;
using ModCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configUtil = new ConfigService();
            var config = configUtil.GetConfig();

            // Pre-construct client, commands, interactivity
            services.AddSingleton(x => new DiscordClient(new DiscordConfiguration()
            {
                LoggerFactory = x.GetRequiredService<ILoggerFactory>(),
                Token = config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Trace
            }));

            services.AddSingleton(x => x.GetRequiredService<DiscordClient>().UseInteractivity(new DSharpPlus.Interactivity.InteractivityConfiguration()
            {
            }));

            services.AddSingleton(x => x.GetRequiredService<DiscordClient>().UseCommandsNext(new CommandsNextConfiguration()
            {
                Services = x.GetRequiredService<IServiceProvider>(),
                EnableMentionPrefix = true,
                StringPrefixes = new string[] { config.DefaultPrefix }
            }));

            var dbService = new DatabaseService(configUtil);
            dbService.Connect();
            services.AddSingleton(dbService);

            // Here we define our dependencies.
            services.AddRazorPages();

            // Config comes first, then the bot gets to initialize
            services.AddSingleton(configUtil);

            services.AddHostedService<BotService>();

            // After that, initialize services that (ab)use DiscordClient, CommandsNextExtension, InteractivityExtension
            services.AddSingleton<TimerService>();
            services.AddHostedService(x => x.GetRequiredService<TimerService>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
