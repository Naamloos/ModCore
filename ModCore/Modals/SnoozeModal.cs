using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Listeners;
using ModCore.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("snz")]
    public class SnoozeModal : IModal
    {
        [ModalField("Snooze for how long?", "timespan", "Default: 15 minutes")]
        public string Timespan { get; set; }

        [ModalHiddenField("msg")]
        public string MessageId { get; set; }

        private static Regex snoozeRegex = new Regex(@"", RegexOptions.Compiled);

        private DatabaseContextBuilder _db;
        private DiscordClient client;
        private SharedData shared;

        public SnoozeModal(DatabaseContextBuilder db, DiscordClient client, SharedData shared)
        {
            _db = db;
            this.client = client;
            this.shared = shared;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            if (ulong.TryParse(MessageId, out var messageId))
            {
                var message = await interaction.Channel.GetMessageAsync(messageId);
                var text = message.Embeds[0].Fields[0].Value;
                var duration = TimeSpan.FromMinutes(15);

                if (!string.IsNullOrEmpty(Timespan))
                {
                    var (realDuration, _) = Dates.ParseTime(Timespan);
                    duration = realDuration;
                }

                if (duration > TimeSpan.FromDays(365)) // 1 year is the maximum
                {
                    await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    {
                        Content = "⚠️ Maximum allowed snooze time span is 1 year.",
                        IsEphemeral = true
                    });
                    return;
                }

                var now = DateTimeOffset.UtcNow;
                var dispatchAt = now + duration;

                // create a new timer
                var reminder = new DatabaseTimer
                {
                    GuildId = (long)interaction.Channel.Guild.Id,
                    ChannelId = (long)interaction.Channel.Id,
                    UserId = (long)interaction.User.Id,
                    DispatchAt = dispatchAt.LocalDateTime,
                    ActionType = TimerActionType.Reminder
                };

                var snoozeContext = "";
                if(message.Components.Count > 0 && message.Components.First().Components.Count == 2)
                {
                    var links = message.Components.First().Components.Where(x => x.GetType().IsAssignableTo(typeof(DiscordLinkButtonComponent))).Cast<DiscordLinkButtonComponent>();
                    if(links.Any())
                    {
                        snoozeContext = links.First().Url;
                    }
                }

                reminder.SetData(new TimerReminderData 
                { 
                    ReminderText = text, 
                    MessageId = messageId, 
                    Snoozed = true,
                    SnoozedContext = snoozeContext
                });

                await using (var db = this._db.CreateContext())
                {
                    db.Timers.Add(reminder);
                    await db.SaveChangesAsync();
                }

                // reschedule timers
                await Timers.ScheduleNextAsync();

                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                {
                    Content = $"⏰ Ok, snoozed to remind you <t:{DateTimeOffset.Now.Add(duration).ToUnixTimeSeconds()}:R> about the following:\n\n{text}",
                    IsEphemeral = true
                });
            }
        }
    }
}
