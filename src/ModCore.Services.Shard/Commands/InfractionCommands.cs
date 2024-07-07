using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Helpers;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using ModCore.Common.Utils;
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

        [SlashCommand("infractions", "Lists user infractions", permissions: Permissions.BanMembers)]
        public async ValueTask ListInfractionsAsync(SlashCommandContext context,
            [Option("user", "ID of the user to list infractions for", ApplicationCommandOptionType.User)]Snowflake user_id)
        {
            var fetchedUser = await _cache.GetFromCacheOrRest<User>(user_id, (rest, id) => rest.GetUserAsync(id));
            if(!fetchedUser.Success)
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, 
                    InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                {
                        Flags = MessageFlags.Ephemeral,
                        Content = "🚫 Failed to fetch user data."
                });
                return;
            }

            var infractionHelper = new InfractionHelper(_database, user_id, context.EventData.GuildId.Value);
            var infractions = await infractionHelper.GetInfractionsAsync();
            if (!infractions.Any())
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token,
                    InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                    {
                        Flags = MessageFlags.Ephemeral,
                        Content = "🚫 User has no infractions!"
                    });
                return;
            }

            // infractions exist, let's list them
            var embed = new Embed()
            {
                Author = new EmbedAuthor()
                {
                    Name = "🚫 Infractions for " + fetchedUser.Value!.Username,
                    IconUrl = fetchedUser.Value.AvatarUrl
                },
                Color = ColorConverter.FromHex("#FF0000"),
                Fields = infractions.Select(x => new EmbedField()
                {
                    Name = $"`{x.Id}`: **{x.Type}**",
                    Value = x.Reason
                }).ToList()
            };

            await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token,
                InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                {
                    Flags = MessageFlags.Ephemeral,
                    Embeds = new[] { embed }
                });
        }

        [SlashCommand("warn", "Adds a warning to a user (with DM)", permissions: Permissions.BanMembers)]
        public async ValueTask AddNoteAsync(SlashCommandContext context,
            [Option("user", "ID of the user to list infractions for", ApplicationCommandOptionType.User)] Snowflake user_id,
            [Option("warning", "Optional text content of this warning", ApplicationCommandOptionType.String)] Optional<string> content)
        {
            var fetchedUser = await _cache.GetFromCacheOrRest<User>(user_id, (rest, id) => rest.GetUserAsync(id));
            if (!fetchedUser.Success)
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token,
                    InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                    {
                        Flags = MessageFlags.Ephemeral,
                        Content = "🚫 Failed to fetch user data."
                    });
                return;
            }

            var infractionHelper = new InfractionHelper(_database, user_id, context.EventData.GuildId.Value);
            await infractionHelper.CreateInfractionAsync(InfractionType.Warning, 
                context.EventData.Member.Value.User.Value.Id, content.HasValue? content : "❌ No reasopn given.", true);

            await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token,
                InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                {
                    Flags = MessageFlags.Ephemeral,
                    Content = $"Warned user <@{user_id}>" + (content.HasValue? $" for: ```{content.Value}```" : "")
                });
        }
    }
}
