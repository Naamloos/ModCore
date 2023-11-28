using DeepL;
using DeepL.Model;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.AutoComplete
{
    public class DeepLLanguageAutoComplete : IAutocompleteProvider
    {
        private static TargetLanguage[] _langs = null;

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var interactions = ctx.Client.GetInteractionExtension();
            var settings = interactions.Services.GetService(typeof(Settings)) as Settings;

            if(_langs == null)
            {
                var translator = new Translator(settings.DeepLToken);
                _langs = await translator.GetTargetLanguagesAsync();
            }

            return _langs
                .Select(x => new DiscordAutoCompleteChoice(x.Name, x.Code))
                .Where(x => string.IsNullOrEmpty((string)ctx.OptionValue) || x.Name.ToLower().Contains(((string)ctx.OptionValue).ToLower()) 
                    || ((string)x.Value).ToLower().Contains(((string)ctx.OptionValue).ToLower()));
        }
    }
}
