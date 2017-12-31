using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic;

namespace ModCore.Commands
{
    [Group("reminder", CanInvokeWithoutSubcommand = true), Aliases("remindme"), Description("Commands for managing your reminders.")]
    public class Reminders
    {
        private const string ReminderTut = @"
Sets a new reminder. The time span parser is fluent and will understand many different formats of reminders:

<Time span>, <Message>
[in] <Time span> to <Message>
<Short time span> <Message> 

Where ""Time span"" represents a time period or a relative date (optionally with time of day specified), such as
\* Tomorrow at 2:00 PM Chicago time
\* Next fortnight
\* 8 hours 20 minutes

A ""Short time span"" represents a time span represented in a single word (we call this a token), such as
\* 2h
\* 4m5s
\* 15min
\* 30min55sec

See these examples:

```
+remindme next week to walk the dog
+remindme tomorrow, fix socket
+remindme 2h5m watch new stranger things episode
+remindme 8 hours wake up
+remindme in an hour to eat something
+remindme in nine months to have a baby
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

        [Description(ReminderTut)]
        public async Task ExecuteGroupAsync(CommandContext ctx, [RemainingText, Description("When the reminder is to be sent.")] string dataToParse)
        {
            await SetAsync(ctx, dataToParse);
        }

        [Command("list"), Description("Lists your active reminders.")]
        public async Task ListAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DatabaseTimer[] reminders;
            using (var db = this.Database.CreateContext())
                reminders = db.Timers.Where(xt =>
                    xt.ActionType == TimerActionType.Reminder && xt.GuildId == (long)ctx.Guild.Id &&
                    xt.UserId == (long)ctx.User.Id).ToArray();
            if (!reminders.Any())
            {
                await ctx.RespondAsync("You have no reminders set.");
                return;
            }

            var rms = reminders.OrderByDescending(xt => xt.DispatchAt).ToArray();
            var interactivity = this.Interactivity;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");

            var page = 1;
            var total = rms.Length / 5 + (rms.Length % 5 == 0 ? 0 : 1);
            var pages = new List<Page>();
            var cembed = new DiscordEmbedBuilder
            {
                Title = $"{emoji} Your currently set reminders:",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Page {page} of {total}"
                }
            };
            foreach (var xr in rms)
            {
                var data = xr.GetData<TimerReminderData>();
                var note = data.ReminderText;
                if (note.Contains('\n'))
                    note = string.Concat(note.Substring(0, note.IndexOf('\n')), "...");

                cembed.AddField(
                    $"In {(DateTimeOffset.UtcNow - xr.DispatchAt).Humanize(4, minUnit: TimeUnit.Second)} (ID: #{xr.Id})",
                    $"{note}");
                if (cembed.Fields.Count < 5) continue;
                page++;
                pages.Add(new Page {Embed = cembed.Build()});
                cembed = new DiscordEmbedBuilder
                {
                    Title = $"{emoji} Your currently set reminders:",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Page {page} of {total}"
                    }
                };
            }
            if (cembed.Fields.Count > 0)
                pages.Add(new Page {Embed = cembed.Build()});

            if (pages.Count > 1)
                await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, pages);
            else
                await ctx.RespondAsync(embed: pages.First().Embed);
        }

        [Command("set"), Description(ReminderTut)]
        public async Task SetAsync(CommandContext ctx, [Description("When the reminder is to be sent"), RemainingText] string dataToParse)
        {
            await ctx.TriggerTypingAsync();

            var (duration, text) = await Dates.ParseTime(dataToParse);
            if (duration == Dates.ParsingError)
            {
                /* await ctx.RespondAsync(
                     $"Sorry, there was an error parsing your reminder.\nIf you see a developer, this info might help them: \n```\n{text}```");
                     */
                await ctx.RespondAsync("Sorry, there was an error parsing your reminder.");
                return;
            }

            if (string.IsNullOrWhiteSpace(text) || text.Length > 128)
            {
                await ctx.RespondAsync(
                    "Reminder text must to be no longer than 128 characters, not empty and not whitespace.");
                return;
            }
#if !DEBUG
            if (duration < TimeSpan.FromSeconds(30))
            {
                await ctx.RespondAsync("Minimum required time span to set a reminder is 30 seconds.");
                return;
            }
#endif

            if (duration > TimeSpan.FromDays(365)) // 1 year is the maximum
            {
                await ctx.RespondAsync("Maximum allowed time span to set a reminder is 1 year.");
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + duration;

            // create a new timer
            var reminder = new DatabaseTimer
            {
                GuildId = (long) ctx.Guild.Id,
                ChannelId = (long) ctx.Channel.Id,
                UserId = (long) ctx.User.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Reminder
            };
            reminder.SetData(new TimerReminderData {ReminderText = text});
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            // reschedule timers
            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);
            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");
            await ctx.RespondAsync(
                $"{emoji} Ok, in {duration.Humanize(4, minUnit: TimeUnit.Second)} I will remind you about the following:\n\n{text}");
        }

        [Command("stop"), Aliases("unset", "remove"), Description("Stops and removes a reminder.")]
        public async Task UnsetAsync(CommandContext ctx, [Description("Which timer to stop. To get a Timer ID, use " +
                                                                        "the `reminder list` command.")] int timerId)
        {
            await ctx.TriggerTypingAsync();

            // find the timer
            var reminder = Timers.FindTimer(timerId, TimerActionType.Reminder, ctx.User.Id, this.Database);
            if (reminder == null)
            {
                await ctx.RespondAsync($"Timer with specified ID (#{timerId}) was not found.");
                return;
            }

            // unschedule and reset timers
            await Timers.UnscheduleTimerAsync(reminder, ctx.Client, this.Database, this.Shared);

            var duration = reminder.DispatchAt - DateTimeOffset.Now;
            var data = reminder.GetData<TimerReminderData>();
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ballot_box_with_check:");
            await ctx.RespondAsync(
                $"{emoji} Ok, timer #{reminder.Id} due in {duration.Humanize(4, minUnit: TimeUnit.Second)} was removed. The reminder's message was:\n\n{data.ReminderText}");
        }

        [Command("clear"), Description("Clears all active reminders.")]
        public async Task ClearAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            await ctx.RespondAsync("Are you sure you want to clear all your active reminders? This action cannot be undone!");

            var m = await this.Interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m == null)
            {
                await ctx.RespondAsync("Timed out.");
            }
            else if (InteractivityUtil.Confirm(m))
            {
                await ctx.RespondAsync("Brace for impact!");
                await ctx.TriggerTypingAsync();
                using (var db = this.Database.CreateContext())
                {
                    List<DatabaseTimer> timers = db.Timers.Where(xt => xt.ActionType == TimerActionType.Reminder && xt.UserId == (long)ctx.User.Id).ToList();

                    var count = timers.Count;
                    await Timers.UnscheduleTimersAsync(timers, ctx.Client, this.Database, this.Shared);


                    await ctx.RespondAsync("Alright, cleared " + count + " timers.");
                }

            }
            else
            {
                await ctx.RespondAsync("Never mind then, maybe next time.");
            }
        }

        [Command("test"), Description("WIP.")]
        public async Task TestAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Timer will dispatch at: `{Shared.TimerData.DispatchTime}`, and has the message ```{Shared.TimerData.DbTimer.GetData<TimerReminderData>().ReminderText}```.");
        }
    }
}
