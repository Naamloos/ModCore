using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using ModCore.Services.Consumer.Interactions.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static ModCore.Common.Utils.DiscordFormatter;

namespace ModCore.Services.Consumer.Interactions.Remind
{
    public class RemindMeCommand : BaseApplicationSubcommand
    {
        public override string ParentName => "remind";

        public override string Name => "me";

        public override string Description => "Sets a new reminder";

        private readonly DiscordRest _rest;

        public RemindMeCommand(DiscordRest rest)
        {
            _rest = rest;
        }

        [ApplicationCommandHandler]
        public async Task HandleAsync(Interaction interaction,
            [Name("in")]
            [Description("In how long the timer should trigger")]
            string timespan,
            [Description("What the reminder is about")]
            string about
        )
        {
            var (duration, _) = DateHelper.ParseTime(timespan);

            var response = new MessageBuilder()
                .AddContainer(container =>
                {
                    container.AddText($"You will be reminded in {DiscordFormatter.Timestamp(duration, TimestampFormat.RelativeTime)}!");
                    container.AddText($"```\n{about.InCodeBlock()}\n```");
                })
                .WithFlags(MessageFlags.Ephemeral)
                .BuildInteractionResponse();

            await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, Common.Discord.Entities.Enums.InteractionResponseType.ChannelMessageWithSource, response);
        }
    }
}
