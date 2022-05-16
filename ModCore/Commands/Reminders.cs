using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.EventHandling;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("reminder"), Aliases("remindme", "remind"), Description("Commands for managing your reminders."), CheckDisable]
    public class Reminders : BaseCommandModule
	{
        private const string ReminderTut = @"
Sets a new reminder. The time span parser is fluent and will understand many different formats of time, as long as they follow the base:

[in] <Time span> [to] [Message]

Where ""Time span"" represents a time period such as
\* Tomorrow
\* Next fortnight
\* 8 hours 20 minutes
\* 2h
\* 4m5s
\* 15min
\* 30min55sec

See these examples:

```
+remindme next week to walk the dog
+remindme tomorrow fix socket
+remindme 2h5m watch new stranger things episode
+remindme 8 hours wake up
+remindme in an hour to eat something
+remindme in nine months to have a baby
+remindme in 7 minutes
+remindme 7m
```

Note that even if your arguments don't fit the grammar denoted above, they might still be parsed fine.
If in doubt, just try it! You can always clear the reminders later. 
";
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }

        public Reminders(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Database = db;
            this.Interactivity = interactive;
        }

        [GroupCommand, Description(ReminderTut)]
        public async Task ExecuteGroupAsync(CommandContext context, [RemainingText, Description("When the reminder is to be sent.")] string data)
        {
            await SetAsync(context, data);
        }

        [Command("list"), Description("Lists your active reminders."), CheckDisable]
        public async Task ListAsync(CommandContext context)
        {
            await context.TriggerTypingAsync();
            DatabaseTimer[] reminders;

            using (var db = this.Database.CreateContext())
                reminders = db.Timers.Where(xt =>
                    xt.ActionType == TimerActionType.Reminder && xt.GuildId == (long)context.Guild.Id &&
                    xt.UserId == (long)context.User.Id).ToArray();
            if (!reminders.Any())
            {
                await context.ElevatedRespondAsync("You have no reminders set.");
                return;
            }

            var orderedreminders = reminders.OrderByDescending(xt => xt.DispatchAt).ToArray();
            var interactivity = this.Interactivity;
            var emoji = DiscordEmoji.FromName(context.Client, ":alarm_clock:");

            var page = 1;
            var total = orderedreminders.Length / 5 + (orderedreminders.Length % 5 == 0 ? 0 : 1);
            var pages = new List<Page>();
            var currentembed = new DiscordEmbedBuilder
            {
                Title = $"{emoji} Your currently set reminders:",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Page {page} of {total}"
                }
            };
            foreach (var reminder in orderedreminders)
            {
                var data = reminder.GetData<TimerReminderData>();
                var note = data.ReminderText;
                if (note.Contains('\n'))
                    note = string.Concat(note.Substring(0, note.IndexOf('\n')), "...");
				note.BreakMentions();


				currentembed.AddField(
                    $"In {(DateTimeOffset.UtcNow - reminder.DispatchAt).Humanize(4, minUnit: TimeUnit.Second)} (ID: #{reminder.Id})",
                    $"{note}");
                if (currentembed.Fields.Count < 5) continue;
                page++;
                pages.Add(new Page("", currentembed));
                currentembed = new DiscordEmbedBuilder
                {
                    Title = $"{emoji} Your currently set reminders:",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Page {page} of {total}"
                    }
                };
            }
            if (currentembed.Fields.Count > 0)
                pages.Add(new Page("", currentembed));

            if (pages.Count > 1)
                await interactivity.SendPaginatedMessageAsync(context.Channel, context.User, pages.ToArray(), new PaginationEmojis());
            else
                await context.ElevatedRespondAsync(embed: pages.First().Embed);
        }

        [Command("set"), Description(ReminderTut), CheckDisable]
        public async Task SetAsync(CommandContext context, [Description("When the reminder is to be sent"), RemainingText] string data)
        {
            await context.TriggerTypingAsync();

            var (duration, text) = Dates.ParseTime(data);
            
            if (string.IsNullOrWhiteSpace(text) || text.Length > 128)
            {
                await context.ElevatedRespondAsync(
                    "⚠️ Reminder text must to be no longer than 128 characters, not empty and not whitespace.");
                return;
            }
#if !DEBUG
            if (duration < TimeSpan.FromSeconds(30))
            {
                await context.ElevatedRespondAsync("Minimum required time span to set a reminder is 30 seconds.");
                return;
            }
#endif

            if (duration > TimeSpan.FromDays(365)) // 1 year is the maximum
            {
                await context.ElevatedRespondAsync("⚠️ Maximum allowed time span to set a reminder is 1 year.");
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + duration;

            // create a new timer
            var reminder = new DatabaseTimer
            {
                GuildId = (long) context.Guild.Id,
                ChannelId = (long) context.Channel.Id,
                UserId = (long) context.User.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Reminder
            };
            reminder.SetData(new TimerReminderData {ReminderText = text, MessageId = context.Message.Id});
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            // reschedule timers
            await Timers.RescheduleTimers(context.Client, this.Database, this.Shared);
            await context.SafeRespondAsync(
                $"⏰ Ok, in {duration.Humanize(4, minUnit: TimeUnit.Second)} I will remind you about the following:\n\n{text.BreakMentions()}");
        }

        [Command("stop"), Aliases("unset", "remove"), Description("Stops and removes a reminder."), CheckDisable]
        public async Task UnsetAsync(CommandContext context, [Description("Which timer to stop. To get a Timer ID, use " +
                                                                        "the `reminder list` command.")] int timerId)
        {
            await context.TriggerTypingAsync();

            // find the timer
            var reminder = Timers.FindTimer(timerId, TimerActionType.Reminder, context.User.Id, this.Database);
            if (reminder == null)
            {
                await context.SafeRespondAsync($"⚠️ Timer with specified ID (#{timerId}) was not found.");
                return;
            }

            // unschedule and reset timers
            await Timers.UnscheduleTimerAsync(reminder, context.Client, this.Database, this.Shared);

            var duration = reminder.DispatchAt - DateTimeOffset.Now;
            var data = reminder.GetData<TimerReminderData>();
            await context.SafeRespondAsync(
                $"✅ Ok, timer #{reminder.Id} due in {duration.Humanize(4, minUnit: TimeUnit.Second)} was removed. The reminder's message was:\n\n{data.ReminderText.BreakMentions()}");
        }

        [Command("clear"), Description("Clears all active reminders."), CheckDisable]
        public async Task ClearAsync(CommandContext context)
        {
            await context.TriggerTypingAsync();

            await context.SafeRespondUnformattedAsync("❓ Are you sure you want to clear all your active reminders? This action cannot be undone!");

            var message = await this.Interactivity.WaitForMessageAsync(x => x.ChannelId == context.Channel.Id && x.Author.Id == context.Member.Id, TimeSpan.FromSeconds(30));

            if (message.TimedOut)
            {
                await context.SafeRespondUnformattedAsync("⚠️⌛ Timed out.");
            }
            else if (InteractivityUtil.Confirm(message.Result))
            {
                await context.SafeRespondUnformattedAsync("🔥 Brace for impact!");
                await context.TriggerTypingAsync();
                using (var db = this.Database.CreateContext())
                {
                    List<DatabaseTimer> timers = db.Timers.Where(xt => xt.ActionType == TimerActionType.Reminder && xt.UserId == (long)context.User.Id).ToList();

                    var count = timers.Count;
                    await Timers.UnscheduleTimersAsync(timers, context.Client, this.Database, this.Shared);


                    await context.SafeRespondUnformattedAsync("✅ Alright, cleared " + count + " timers.");
                }

            }
            else
            {
                await context.SafeRespondUnformattedAsync("🤷 Never mind then, maybe next time.");
            }
        }

        [Command("test"), Description("WIP."), RequireOwner]
        public async Task TestAsync(CommandContext context)
        {
            await context.SafeRespondAsync($"❓ Timer will dispatch at: `{Shared.TimerData.DispatchTime}`, and has the message ```{Shared.TimerData.DbTimer.GetData<TimerReminderData>().ReminderText.BreakMentions()}```.");
        }
    }
}
