using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    [Group("reminder"), Description("Commands for managing your reminders.")]
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

        [Command("list"), Description("Lists your active reminders.")]
        public async Task ListAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var db = this.Database.CreateContext();
            var reminders = db.Timers.Where(xt => xt.ActionType == TimerActionType.Reminder && xt.GuildId == (long)ctx.Guild.Id && xt.UserId == (long)ctx.User.Id);
            if (!reminders.Any())
            {
                await ctx.RespondAsync("You have no reminders set.");
                return;
            }

            var rms = reminders.OrderByDescending(xt => xt.DispatchAt).ToArray();
            var interactivity = this.Interactivity;
            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");
            //var cal = DiscordEmoji.FromName(ctx.Client, ":calendar:");
            //var ntd = DiscordEmoji.FromName(ctx.Client, ":notepad_spiral:");

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

                cembed.AddField($"In {(DateTimeOffset.UtcNow - xr.DispatchAt).Humanize(4, minUnit: TimeUnit.Second)}", $"{note}", false);
                if (cembed.Fields.Count >= 5)
                {
                    cpnum++;
                    pages.Add(new Page { Embed = cembed.Build() });
                    cembed = new DiscordEmbedBuilder
                    {
                        Title = $"{emoji} Your currently set reminders:",
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = $"Page {cpnum} of {tpnum}"
                        }
                    };
                }
            }
            if (cembed.Fields.Count > 0)
                pages.Add(new Page { Embed = cembed.Build() });

            if (pages.Count > 1)
                await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, pages);
            else
                await ctx.RespondAsync(embed: pages.First().Embed);

            /*var sb = new StringBuilder();
            sb.Append(emoji).AppendLine(" Your currently set reminders:").AppendLine();
            foreach (var xr in rms)
            {
                var data = xr.GetData<TimerReminderData>();
                var note = data.ReminderText;
                if (note.Contains('\n'))
                    note = string.Concat(note.Substring(0, note.IndexOf('\n')), "...");

                sb.Append(cal).Append(" At ").AppendLine(xr.DispatchAt.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                sb.Append(ntd).Append(" ").AppendLine(note);
                sb.AppendLine();
            }
            var str = sb.ToString().Trim().Replace("\r\n", "\n");

            await ctx.RespondAsync(str);*/
        }

        [Command("set"), Description("Sets a new reminder.")]
        public async Task SetAsync(CommandContext ctx, [Description("After how much time to set the timer off.")] TimeSpan duration, [RemainingText, Description("Reminder text.")] string text)
        {
            await ctx.TriggerTypingAsync();

            if (text.Length > 128)
            {
                await ctx.RespondAsync("Reminder text needs to be no longer than 128 characters.");
                return;
            }

            if (duration < TimeSpan.FromSeconds(30))
            {
                await ctx.RespondAsync("Minimum required time span to set a reminder is 30 seconds.");
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var dispatch_at = now + duration;

            // create a new timer
            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatch_at.LocalDateTime,
                ActionType = TimerActionType.Reminder
            };
            reminder.SetData(new TimerReminderData { ReminderText = text });
            var db = this.Database.CreateContext();
            db.Timers.Add(reminder);
            await db.SaveChangesAsync();

            // reschedule timers
            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

            var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");
            await ctx.RespondAsync($"{emoji} Ok, in {duration.Humanize(4, minUnit: TimeUnit.Second)} I will remind you about the following:\n\n{text}");
        }
    }
}
