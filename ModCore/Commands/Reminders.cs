﻿using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.SlashCommands;
using Humanizer;
using Humanizer.Localisation;
using ModCore.AutoComplete;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [SlashCommandGroup("remind", "Reminder-related commands")]
    [GuildOnly]
    public class Reminders : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }
        public SharedData Shared { private get; set; }
        public InteractivityExtension Interactivity { private get; set; }

        [SlashCommand("me", "Sets a reminder in ModCore")]
        public async Task SetAsync(InteractionContext ctx, 
            [Option("in", "In how long the reminder should trigger.")]string timespan,
            [Option("about", "What to remind about.")]string about)
        {
            var (duration, _) = (TimeSpan.FromSeconds(0), "");

            try
            {
                (duration, _) = Dates.ParseTime(timespan);
            }catch(Exception)
            {
                await ctx.CreateResponseAsync(
                    "⚠️ Unable to parse your reminder duration. Please try again.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(about) || about.Length > 128)
            {
                await ctx.CreateResponseAsync(
                    "⚠️ Reminder text must to be no longer than 128 characters, not empty and not whitespace.", true);
                return;
            }

            if (duration > TimeSpan.FromDays(365)) // 1 year is the maximum
            {
                await ctx.CreateResponseAsync("⚠️ Maximum allowed time span to set a reminder is 1 year.", true);
                return;
            }

            if (duration < TimeSpan.FromSeconds(15)) // 1 year is the maximum
            {
                await ctx.CreateResponseAsync("⚠️ Minimum allowed time span to set a reminder is 15 seconds.", true);
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + duration;

            // create a new timer
            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Reminder
            };
            reminder.SetData(new TimerReminderData { ReminderText = about});
            await using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            // reschedule timers
            await Timers.ScheduleNextAsync();
            await ctx.CreateResponseAsync(
                $"⏰ Ok, <t:{DateTimeOffset.Now.Add(duration).ToUnixTimeSeconds()}:R> I will remind you about the following:\n\n{about}", true);
        }

        [SlashCommand("list", "Lists all reminders you have set.")]
        public async Task ListAsync(InteractionContext ctx)
        {
            DatabaseTimer[] reminders;

            await using (var db = this.Database.CreateContext())
                reminders = db.Timers.Where(xt =>
                    xt.ActionType == TimerActionType.Reminder &&
                    xt.UserId == (long)ctx.User.Id).ToArray();
            if (!reminders.Any())
            {
                await ctx.CreateResponseAsync("You have no reminders set.", true);
                return;
            }

            var orderedreminders = reminders.OrderByDescending(xt => xt.DispatchAt).ToArray();
            var interactivity = this.Interactivity;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");

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

                currentembed.AddField(
                    $"<t:{new DateTimeOffset(reminder.DispatchAt).ToUnixTimeSeconds()}:R> in <#{reminder.ChannelId}> (ID: #{reminder.Id})",
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

            await interactivity.SendPaginatedResponseAsync(ctx.Interaction, true, ctx.User, pages.ToArray(), deletion: ButtonPaginationBehavior.DeleteButtons);
        }

        [SlashCommand("stop", "Stops a specific reminder by it's ID.")]
        public async Task StopAsync(InteractionContext ctx,
            [Option("id", "Reminder ID", true)][Autocomplete(typeof(ReminderIdAutoComplete))][Range(0, int.MaxValue)] string id)
        {
            var parsed = int.TryParse(id, out int result);
            DatabaseTimer reminder = null;
            // find the timer
            if(parsed)
                reminder = Timers.FindTimer(result, TimerActionType.Reminder, ctx.User.Id, this.Database);

            if (parsed && reminder == null)
            {
                await ctx.CreateResponseAsync($"⚠️ Timer with specified ID (#{id}) was not found.", true);
                return;
            }

            // unschedule and reset timers
            await Timers.UnscheduleTimersAsync(reminder);

            var duration = reminder.DispatchAt - DateTimeOffset.Now;
            var data = reminder.GetData<TimerReminderData>();
            await ctx.CreateResponseAsync(
                $"✅ Ok, timer #{reminder.Id} due in {duration.Humanize(4, minUnit: TimeUnit.Second)} was removed. " +
                $"The reminder's message was:\n\n{data.ReminderText.BreakMentions()}", true);
        }

        [SlashCommand("clear", "Clears all of your reminders.")]
        public async Task ClearAsync(InteractionContext ctx)
        {
            var confirm = new DiscordFollowupMessageBuilder()
                .WithContent("❓ Are you sure you want to clear all your active reminders? This action cannot be undone!")
                .AsEphemeral();

            var confirmed = await ctx.ConfirmAsync(confirm, "Yes, get rid of it!", "Never mind.", DiscordEmoji.FromUnicode("💣"));

            if (confirmed.TimedOut)
            {
                await ctx.EditFollowupAsync(confirmed.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("⚠️⌛ Timed out."));
            }
            else if(confirmed.Accepted)
            {
                await using var db = this.Database.CreateContext();
                DatabaseTimer[] timers = db.Timers.Where(xt => xt.ActionType == TimerActionType.Reminder && xt.UserId == (long)ctx.User.Id).ToArray();

                var count = timers.Length;
                await Timers.UnscheduleTimersAsync(timers);

                await ctx.EditFollowupAsync(confirmed.FollowupMessage.Id, new DiscordWebhookBuilder()
                    .WithContent("✅ Alright, cleared " + count + $" timer{(count > 1? "s" : "")}."));
            }
            else
            {
                await ctx.EditFollowupAsync(confirmed.FollowupMessage.Id, new DiscordWebhookBuilder().WithContent("🤷 Never mind then, maybe next time."));
            }
        }
    }
}
