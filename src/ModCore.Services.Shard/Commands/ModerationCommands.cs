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
using ModCore.Common.Database.Helpers;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;

namespace ModCore.Services.Shard.Commands
{
    public class ModerationCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;
        private readonly CacheService _cache;
        private readonly DatabaseContext _database;

        public ModerationCommands(ILogger<ModerationCommands> logger, CacheService cache, DatabaseContext database)
        {
            _logger = logger;
            _cache = cache;
            _database = database;
        }

        [SlashCommand("ban", "Bans a user", permissions: Permissions.BanMembers)]
        public async ValueTask BanAsync(
            SlashCommandContext context,
            [Option("user", "User to ban", ApplicationCommandOptionType.User)] Snowflake userToBan, 
            [Option("reason", "Reason to ban user", ApplicationCommandOptionType.String)] Optional<string> reason, 
            [Option("notify", "Whether to notify said user", ApplicationCommandOptionType.Boolean)] Optional<bool> notify)
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
                    Content = $"You were banned from {fetchGuild.Value.Name}.\nReason:\n```\n{givenReason}\n```" +
                        $"This server does **not** have appeals enabled."
                });
                sentDM = dm.Success;
            }

            var ban = await context.RestClient.CreateGuildBanAsync(context.EventData.GuildId, userToBan);

            if (ban.Success)
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                    new InteractionMessageResponse()
                    {
                        Content = $"Succesfully banned member <@{userToBan}>. Sent DM: {(sentDM ? "Yes" : "No")}.\n" +
                        $"Reason:\n```\n{givenReason}\n```\n",
                        Flags = MessageFlags.Ephemeral
                    });

                var infractionHelper = new InfractionHelper(_database, userToBan, context.EventData.GuildId.Value);
                await infractionHelper.CreateInfractionAsync(InfractionType.Ban, context.EventData.Member.Value.User.Value.Id, givenReason, sentDM);
            }
            else
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                    new InteractionMessageResponse()
                    {
                        Content = $"Failed banning <@{userToBan}>. " +
                        $"{(sentDM ? "Already sent a DM?" : "")}.\n",
                        Flags = MessageFlags.Ephemeral
                    });
            }
        }

        [SlashCommand("hackban", "HackBans a user, by their ID.", permissions: Permissions.BanMembers)]
        public async ValueTask HackBanAsync(SlashCommandContext context, 
            [Option("user_id", "ID of the user to hackban", ApplicationCommandOptionType.String)]string id)
        {
            if(!ulong.TryParse(id, out var userId))
            {
                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                    new InteractionMessageResponse()
                    {
                        Content = $"⚠️ Your ID input could not be converted to a valid User ID!",
                        Flags = MessageFlags.Ephemeral
                    });
                return;
            }

            await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, InteractionResponseType.ChannelMessageWithSource,
                new InteractionMessageResponse()
                {
                    Content = $"⚠️ Not implemented yet! {userId}",
                    Flags = MessageFlags.Ephemeral
                });
        }
    }
}
