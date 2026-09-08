using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Consumer.Interactions.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Moderation
{
    public class HackBanCommand : BaseApplicationCommand
    {
        public override string Name => "hackban";
        public override string Description => "Ban a user by their ID, even if they are not in the server";
    
        public override Permissions DefaultMemberPermissions => Permissions.BanMembers;

        private readonly DiscordRest _rest;

        public HackBanCommand(DiscordRest rest)
        {
            _rest = rest;
        }

        [ApplicationCommandHandler]
        public async Task ExecuteAsync(Interaction interaction,
            [Description("ID of the user to ban.")]
            [Name("user_id")]
            string userId
            )
        {
            if(ulong.TryParse(userId, out ulong parsedUserId))
            {
                var userSnowflake = new Snowflake(parsedUserId);
                await _rest.CreateGuildBanAsync(interaction.GuildId.Value!, userSnowflake, 0);
                await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource,
                    new MessageBuilder().WithContent($"User with ID {userId} has been banned.").WithFlags(MessageFlags.Ephemeral).BuildInteractionResponse());
            }
            else
            {
                await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource,
                    new MessageBuilder().WithContent($"Invalid user ID: {userId}").WithFlags(MessageFlags.Ephemeral).BuildInteractionResponse());
            }
        }
    }
}
