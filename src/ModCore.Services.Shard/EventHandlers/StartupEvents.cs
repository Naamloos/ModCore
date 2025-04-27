using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.InteractionFramework;
using ModCore.Common.Utils;
using ModCore.Services.Shard.Services;
using System.Reflection;

namespace ModCore.Services.Shard.EventHandlers
{
    /// <summary>
    /// This event handler handles anything that should be ran around startup.
    /// </summary>
    public class StartupEvents : ISubscriber<Ready>, ISubscriber<Hello>, ISubscriber<GuildCreate>
    {
        public Gateway Gateway { get; set; }

        private readonly ILogger _logger;
        private readonly DiscordRest _rest;
        private readonly InteractionService _interactions;
        private readonly TransientService<DatabaseContext> _database;
        private readonly TimerService _timerService;
        private bool initialized = false;

        public StartupEvents(ILogger<StartupEvents> logger, DiscordRest rest, 
            InteractionService interactions, TransientService<DatabaseContext> database, 
            TimerService timerService)
        {
            _logger = logger;
            _rest = rest;
            _interactions = interactions;
            _database = database;
            _timerService = timerService;
        }

        public async ValueTask HandleEvent(Hello data)
        {
            _logger.LogInformation("Hello from event handler!");
            var database = _database.GetTransient();
            var pendingMigrations = await database.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applied pending database migrations: {0}", string.Join(", ", pendingMigrations));
                await database.Database.MigrateAsync();
            }
            else
            {
                _logger.LogInformation("No pending database migrations.");
            }
        }

        public async ValueTask HandleEvent(Ready data)
        {
            _logger.LogInformation("Ready from event handler! User is {0} with ID {1}.",
                data.User.Username, data.User.Id);
            var application = await _rest.GetApplicationAsync(data.Application.Id);

            if (application.Success)
            {
                _logger.LogInformation("Application is registered under ID {0}. Owner username is {1}.", data.Application.Id, application.Value!.Owner!.Value.Username);
                if (!initialized)
                {
                    _interactions.RegisterCommands(Assembly.GetExecutingAssembly());
                    if (data.Shard.HasValue && data.Shard.Value[0] == 0)
                    {
                        // Only send commands if we know we're definitely on shard 0!!
                        await _interactions.PublishCommands(application.Value.Id);
                    }
                }
            }
            else
            {
                _logger.LogCritical("Failed to fetch application info!");
            }

            if (!initialized)
            {
                _interactions.Start(Gateway);
                await _timerService.StartAsync(data.Shard.HasValue? data.Shard.Value[0] : 0);
                initialized = true;
            }
        }

        public async ValueTask HandleEvent(GuildCreate data)
        {
            _logger.LogInformation("Guild {0} has {1} members! Sent from event handler.", data.Name, data.MemberCount);

            var database = _database.GetTransient();
            ulong guildId = data.Id;
            if (!database.Guilds.Any(x => x.GuildId == guildId))
            {
                database.Guilds.Add(new DatabaseGuild()
                {
                    GuildId = guildId
                });
            }

            await database.SaveChangesAsync();
        }
    }
}
