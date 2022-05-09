using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using ModCore.Logic.Utils.Captcha;

namespace ModCore.Commands
{
    [Group("confignew")]
    [Aliases("cfgn")]
    [Description("Guild configuration options. Invoking without a subcommand will list current guild's settings.")]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public partial class ConfigNew : BaseCommandModule
    {
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        private RandomNumberProvider RandomNumberProvider { get; } = new RandomNumberProvider();
        private CaptchaImageProvider CaptchaProvider { get; }
        public SharedData Shared { get; }

        public ConfigNew(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive)
        {
            this.Database = db;
            this.Interactivity = interactive;
            this.CaptchaProvider = new CaptchaImageProvider(RandomNumberProvider);
            this.Shared = shared;
        }

        [GroupCommand, Description("Shows the current config.")]
        public async Task ExecuteGroupAsync(CommandContext context)
        {
            await context.IfGuildSettingsAsync(
                async () => // config does not yet exist
                {
                    await context.RespondAsync($"⚠️ You have not yet set up your server!\nRun `{Shared.DefaultPrefix}config setup` to set up ModCore for your server.");
                },
                async (settings) => // config exists
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                        .WithTitle($"🔧 Server configuration for {context.Guild.Name}.")
                        .WithDescription($"Prefix: {getPrefix(settings)}");

                    await context.RespondAsync(embed);
                }
            );
        }

        #nullable enable
        private string getPrefix(GuildSettings? settings)
        {
            return settings == null? Shared.DefaultPrefix : (string.IsNullOrEmpty(settings.Prefix)? Shared.DefaultPrefix : settings.Prefix);
        }
    }
}
