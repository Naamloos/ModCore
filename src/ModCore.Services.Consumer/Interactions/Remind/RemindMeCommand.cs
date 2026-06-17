using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Timers;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.PubSub;
using ModCore.Common.PubSub.Payloads;
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
        private readonly DatabaseContext _database;
        private readonly PubSubService _pubSub;

        public RemindMeCommand(DiscordRest rest, DatabaseContext database, PubSubService pubSub)
        {
            _rest = rest;
            _database = database;
            _pubSub = pubSub;
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

            // TODO: i18n, value checks

            var newTimer = new DatabaseTimer()
            {
                GuildId = interaction.GuildId.Value,
                Type = TimerTypes.Reminder,
                TriggersAt = DateTime.UtcNow.Add(duration),
            };

            newTimer.SetData<ReminderTimerData>(new()
            {
                ChannelId = interaction.ChannelId.Value,
                CreatedAt = DateTime.UtcNow,
                Text = about,
                UserId = interaction.Member.Value.User.Value.Id,
            });

            var savedTimer = await _database.Timers.AddAsync(newTimer);
            await _database.SaveChangesAsync();

            await _pubSub.PublishAsync(new CreateTimerPayload()
            {
                TimerId = savedTimer.Entity.TimerId // Not necessary anymore, but kept for consistency
            });

            var response = new MessageBuilder()
                .AddContainer(container =>
                {
                    container.AddText($"🔔 You will be reminded in {DiscordFormatter.Timestamp(duration, TimestampFormat.RelativeTime)}!");
                    container.AddText($"```\n{about.InCodeBlock()}\n```");
                })
                .WithFlags(MessageFlags.Ephemeral)
                .BuildInteractionResponse();

            await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource, response);
        }
    }
}
