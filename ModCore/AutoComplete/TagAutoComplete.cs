using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.AutoComplete
{
    public class TagAutoComplete : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            await Task.Yield();

            var interactions = ctx.Client.GetInteractionExtension();
            var database = interactions.Services.GetService(typeof(DatabaseContextBuilder)) as DatabaseContextBuilder;

            await using var db = database.CreateContext();
            var tags = db.Tags.Where(x => x.GuildId == (long)ctx.Guild.Id && x.ChannelId < 1).ToList();
            var channelTags = db.Tags.Where(x => x.ChannelId == (long)ctx.Channel.Id).ToList();

            tags.RemoveAll(x => channelTags.Any(y => y.Name == x.Name));
            tags.AddRange(channelTags);

            return tags.Where(x => x.Name.Contains((string)ctx.OptionValue))
                .OrderBy(x => x.Name.IndexOf((string)ctx.OptionValue))
                .Take(25)
                .Select(x => new DiscordAutoCompleteChoice($"{(x.ChannelId < 1? "🌍": "💬")} {x.Name}", x.Name));
        }
    }
}
