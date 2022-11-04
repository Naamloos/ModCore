using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Interfaces;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Buttons
{
    [Button("cfg_opt_on")]
    public class ConfigValueButton : IButton
    {
        [ButtonField("p")]
        public string SettingPath { get; set; }

        [ButtonField("v")]
        public string Value { get; set; }

        private DatabaseContextBuilder database;

        public ConfigValueButton(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            var context = database.CreateContext();

            var cfg = context.GuildConfig.FirstOrDefault(x => x.GuildId == (long)interaction.GuildId);

            if (cfg == null)
                return;

            var settings = cfg.GetSettings();

            ConfigValueSerialization.SetConfigValue(settings, SettingPath, Value);
            cfg.SetSettings(settings);

            context.GuildConfig.Update(cfg);
            await context.SaveChangesAsync();

            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, 
                new DiscordInteractionResponseBuilder().WithContent($"✅ Set config value `{SettingPath}` to `{Value}`!"));
        }
    }
}
