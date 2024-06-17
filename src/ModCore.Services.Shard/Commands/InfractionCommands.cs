using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Commands
{
    public class InfractionCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;
        private readonly CacheService _cache;
        private readonly DatabaseContext _database;

        public InfractionCommands(ILogger<ModerationCommands> logger, CacheService cache, DatabaseContext database)
        {
            _logger = logger;
            _cache = cache;
            _database = database;
        }

        public async ValueTask ListInfractionsAsync(SlashCommandContext context,
            [Option("user", "ID of the user to list infractions for", ApplicationCommandOptionType.User)]ulong user_id)
        {

        }

        public async ValueTask AddNoteAsync(SlashCommandContext context)
        {

        }
    }
}
