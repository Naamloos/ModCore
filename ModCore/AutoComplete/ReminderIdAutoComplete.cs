using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.AutoComplete
{
    public class ReminderIdAutoComplete : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            await Task.Yield();

            var interactions = ctx.Client.GetInteractionExtension();
            var database = interactions.Services.GetService(typeof(DatabaseContextBuilder)) as DatabaseContextBuilder;

            DatabaseTimer[] reminders;

            await using (var db = database.CreateContext())
                reminders = db.Timers.Where(xt =>
                    xt.ActionType == TimerActionType.Reminder &&
                    xt.UserId == (long)ctx.User.Id).ToArray();

            var selection = reminders.Select(x => (x.Id, x.GetData<TimerReminderData>()));

            var value = (string)ctx.OptionValue;

            return selection.Where(x => x.Item2.ReminderText.Contains(value) || x.Id.ToString().Contains(value))
                .OrderBy(x => x.Id.ToString().Contains(value) ? -2 : x.Item2.ReminderText.IndexOf(value))
                .Take(25)
                .Select(x => new DiscordAutoCompleteChoice($"⏱️ (#{x.Id}) " + (x.Item2.ReminderText.Length > 25 ? x.Item2.ReminderText.Substring(0, 25) : x.Item2.ReminderText), x.Id.ToString()));
        }
    }
}
