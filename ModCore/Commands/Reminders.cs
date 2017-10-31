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

namespace ModCore.Commands
{
    [Group("reminder"), Aliases("remindme"), Description("Commands for managing your reminders.")]
    public class Reminders
    {
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }

        public Reminders(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Database = db;
            this.Interactivity = interactive;
        }

        [Description("Sets a new reminder.")]
        public async Task ExecuteGroupAsync(CommandContext ctx,
            [Description("After how much time to set the timer off.")] TimeSpan duration,
            [RemainingText, Description("Reminder text.")] string text)
        {
            await SetAsync(ctx, duration, text);
        }

        [Command("list"), Description("Lists your active reminders.")]
        public async Task ListAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            DatabaseTimer[] reminders;
            using (var db = this.Database.CreateContext())
                reminders = db.Timers.Where(xt =>
                    xt.ActionType == TimerActionType.Reminder && xt.GuildId == (long) ctx.Guild.Id &&
                    xt.UserId == (long) ctx.User.Id).ToArray();
            if (!reminders.Any())
            {
                await ctx.RespondAsync("You have no reminders set.");
                return;
            }

            var rms = reminders.OrderByDescending(xt => xt.DispatchAt).ToArray();
            var interactivity = this.Interactivity;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");

            var cpnum = 1;
            var tpnum = rms.Length / 5 + (rms.Length % 5 == 0 ? 0 : 1);
            var pages = new List<Page>();
            var cembed = new DiscordEmbedBuilder
            {
                Title = $"{emoji} Your currently set reminders:",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Page {cpnum} of {tpnum}"
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
                cpnum++;
                pages.Add(new Page {Embed = cembed.Build()});
                cembed = new DiscordEmbedBuilder
                {
                    Title = $"{emoji} Your currently set reminders:",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Page {cpnum} of {tpnum}"
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

        [Command("set"), Description("Sets a new reminder.")]
        public async Task SetAsync(CommandContext ctx,
            [Description("After how much time to set the timer off.")] TimeSpan duration,
            [RemainingText, Description("Reminder text.")] string text)
        {
            await ctx.TriggerTypingAsync();

            if (string.IsNullOrWhiteSpace(text) || text.Length > 128)
            {
                await ctx.RespondAsync(
                    "Reminder text needs to be no longer than 128 characters, cannot be null, empty, or whitespace-only.");
                return;
            }

            if (duration < TimeSpan.FromSeconds(30))
            {
                await ctx.RespondAsync("Minimum required time span to set a reminder is 30 seconds.");
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

        [Command("stop"), Description("Stops and removes a timer.")]
        public async Task UnsetAsync(CommandContext ctx, [Description("Which timer to stop.")] int timerId)
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
    }
}