using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Timers;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModCore.Services.Shard.Commands
{
    [SlashCommand("Reminder commands")]
    public class Remind : BaseCommandHandler
    {
        private readonly TimerService _timers;
        private readonly DatabaseContext _databaseContext;

        public Remind(TimerService timers, DatabaseContext databaseContext)
        {
            _timers = timers;
            _databaseContext = databaseContext;
        }

        [SlashCommand("Reminds you at a given time.")]
        public async ValueTask Me(SlashCommandContext ctx, 
            [Option("Time to trigger at", ApplicationCommandOptionType.String)]string time,
            [Option("What to remind you about", ApplicationCommandOptionType.String)]string about)
        {
            var trigger = DateTimeOffset.UtcNow.AddSeconds(5);

            var newTimer = new DatabaseTimer()
            {
                GuildId = ctx.EventData.GuildId.Value.Value,
                ShardId = ctx.Gateway.ShardId,
                TriggersAt = trigger,
                Type = TimerTypes.Reminder
            };

            newTimer.SetData(new ReminderTimerData()
            {
                ChannelId = ctx.EventData.ChannelId.Value.Value,
                UserId = ctx.EventData.Member.Value.User.Value.Id.Value,
                CreatedAt = DateTimeOffset.UtcNow,
                Snoozed = false,
                Text = about
            });
            _databaseContext.Timers.Add(newTimer);
            await _databaseContext.SaveChangesAsync();
            _timers.ScheduleNext();

            var resp = await ctx.RestClient.CreateInteractionResponseAsync(ctx.EventData.Id, ctx.EventData.Token, InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
            {
                Content = $"⏰ Ok, <t:{trigger.ToUnixTimeSeconds()}:R> I will remind you about the following:\n\n{about}",
                Flags = MessageFlags.Ephemeral
            });
        }
    }
}
