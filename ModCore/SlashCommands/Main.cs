using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using ModCore.Entities;
using ModCore.Extensions;
using ModCore.Utils.Extensions;
using ModCore.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    public class Main : ApplicationCommandModule
    {
        public SharedData Shared { private get; set; }

        [SlashCommand("About", "Prints information about ModCore.")]
        public async Task AboutAsync(InteractionContext ctx)
        {
            var eb = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle("ModCore")
                .WithDescription("A powerful moderating bot written on top of DSharpPlus")
                .AddField("Main developer", "[Naamloos](https://github.com/Naamloos)")
                .AddField("Special thanks to these contributors:",
                    "[uwx](https://github.com/uwx), " +
                    "[jcryer](https://github.com/jcryer), " +
                    "[Emzi0767](https://github.com/Emzi0767), " +
                    "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
                    "[DrCreo](https://github.com/DrCreo), " +
                    "[aexolate](https://github.com/aexolate), " +
                    "[Drake103](https://github.com/Drake103) and " +
                    "[Izumemori](https://github.com/Izumemori)")
                .AddField("Environment",
                    $"*OS:* {Environment.OSVersion.VersionString}" +
                    $"\n*Framework:* {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}" +
                    $"\n*DSharpPlus:* {ctx.Client.VersionString}" +
                    $"\n*Servers:* {this.Shared.ModCore.Shards.Select(x => x.Client.Guilds.Count).Sum()}" +
                    $"\n*Shards:* {this.Shared.ModCore.Shards.Count}")
                .AddField("Contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
                .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl)
                .Build();

            var message = new DiscordFollowupMessageBuilder()
                .AddEmbed(eb);

            await ctx.CreateResponseAsync(eb, true);
        }

        [SlashCommand("Test", "Test Command Ignore Me.")]
        public async Task TestAsync(InteractionContext ctx)
        {
            await ctx.Client.GetModalExtension().RespondWithModalAsync<DummyModal>(ctx.Interaction, "Dummy Modal", new Dictionary<string, string>()
            {
                { "hidden", "test hidden" }
            });
        }
    }
}
