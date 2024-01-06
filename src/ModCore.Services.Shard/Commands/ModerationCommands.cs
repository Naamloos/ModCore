using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using ModCore.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Commands
{
    public class ModerationCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;
        private readonly CacheService _cache;

        public ModerationCommands(ILogger<ModerationCommands> logger, CacheService cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [SlashCommand("Bans a user", permissions: Permissions.BanMembers)]
        public async ValueTask Ban(
            SlashCommandContext context,
            [Option("User to ban", ApplicationCommandOptionType.User)] Snowflake userToBan, 
            [Option("Reason to ban user", ApplicationCommandOptionType.String)] Optional<string> reason, 
            [Option("Whether to notify said user", ApplicationCommandOptionType.Boolean)] Optional<bool> notify)
        {
            var fetchGuild = await _cache.GetFromCacheOrRest<Guild>(context.EventData.GuildId, (rest, id) => rest.GetGuildAsync(id));
            if(!fetchGuild.Success)
            {
                // failure! tell user.
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                    new InteractionMessageResponse()
                    {
                        Content = "Something went wrong fetching relevant data for your request!",
                        Flags = MessageFlags.Ephemeral
                    });
                return;
            }

            var dmChannel = await context.RestClient.CreateDMChannelAsync(userToBan);

            var givenReason = reason.HasValue ? reason.Value.Replace("`", "'") : "No reason given.";

            var sentDM = false;
            if(dmChannel.Success)
            {
                var dm = await context.RestClient.CreateMessageAsync(dmChannel.Value.Id, new CreateMessage()
                {
                    Content = $"You were banned from {fetchGuild.Value.Name}.\nReason:\n```\n{givenReason}\n```"
                });
                sentDM = dm.Success;
            }

            await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                new InteractionMessageResponse()
                {
                    Content = $"Succesfully banned member <@{userToBan}>. Sent DM: {(sentDM ? "Yes" : "No")}.\n" +
                    $"Reason:\n```\n{givenReason}\n```",
                    Flags = MessageFlags.Ephemeral
                });
        }
    }
}
