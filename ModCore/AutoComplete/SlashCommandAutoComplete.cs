using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.AutoComplete
{
    public class SlashCommandAutoComplete : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            await Task.Yield();

            var value = ctx.OptionValue as string;

            var slashies = ctx.Client.GetSlashCommands();
            var registered = slashies.RegisteredCommands;
            var commands = registered.First(x => x.Key == null).Value.Where(x => x.Type == ApplicationCommandType.SlashCommand);
            Dictionary<string, ulong> qualifiedCommands = new Dictionary<string, ulong>();

            foreach(var command in commands)
            {
                if(command.Options != null && command.Options.Any(x => x.Type == ApplicationCommandOptionType.SubCommand))
                {
                    foreach(var option in buildSubCommands("/" + command.Name, command.Options))
                    {
                        qualifiedCommands.Add(option, command.Id);
                    }
                }
                else
                {
                    qualifiedCommands.Add("/" + command.Name, command.Id);
                }
            }

            var qualified = qualifiedCommands.Where(x => x.Key.Substring(1).StartsWith(value)).Take(5).Select(x => new DiscordAutoCompleteChoice(x.Key, $"</{x.Key.Substring(1)}:{x.Value}>"));
            return qualified.ToList();
        }

        private string[] buildSubCommands(string parent, IEnumerable<DiscordApplicationCommandOption> options)
        {
            List<string> output = new List<string>();

            foreach(var option in options)
            {
                if(option.Options != null && option.Options.Any(x => x.Type == ApplicationCommandOptionType.SubCommand))
                {
                    output.AddRange(buildSubCommands(parent + " " + option.Name, option.Options));
                }
                else
                {
                    output.Add(parent + " " + option.Name);
                }
            }

            return output.ToArray();
        }
    }
}
