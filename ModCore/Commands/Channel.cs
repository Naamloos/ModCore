using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace ModCore.Commands
{
    [Group("channel", CanInvokeWithoutSubcommand = true)]
    [Aliases("chnl")]
    [Description("Channel options. Invoking without a subcommand will list current channel's settings.")]
    [RequirePermissions(Permissions.ManageChannels)]
    public class Channel
    {
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"#{ctx.Channel.Name} Channel Settings"
            };

            embed.AddField("Channel type", ctx.Channel.Type.ToString(), true);
            embed.AddField("Max. users", FormatMaxUsers(ctx), true);

            await ctx.RespondAsync(embed: embed.Build());
        }

        private static string FormatMaxUsers(CommandContext ctx)
        {
            if (ctx.Channel.UserLimit > 0)
                return ctx.Channel.UserLimit.ToString();

            return "Unlimited";
        }

        [Command("name")]
        [Aliases("n")]
        [Description("Rename this channel.")]
        public async Task NameAsync(CommandContext ctx, [RemainingText] [Description("New channel name.")] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                await ctx.Message.RespondAsync("Please specify name of the channel: \"channel name <newname>\".");
                return;
            }

            if (name == ctx.Channel.Name)
            {
                await ctx.Message.RespondAsync(
                    $"Could not rename the channel. Reason: New name \"{name}\" matches the current name.");
                return;
            }

            var trimmedName = TrimName(name);

            if (trimmedName == ctx.Channel.Name)
            {
                await ctx.Message.RespondAsync(
                    $"Could not rename the channel. Reason: New name \"{name}\" was trimmed to \"{trimmedName}\" and matches the current name.");
                return;
            }

            await ctx.Channel.ModifyAsync(trimmedName);
            await ctx.Message.RespondAsync($"This channel was renamed to \"{trimmedName}\".");
        }

        private static string TrimName(string name)
        {
            name = name.Replace(' ', '-');

            var validNameBuilder = new StringBuilder();

            foreach (var c in name.Where(IsValidChar))
                validNameBuilder.Append(c);

            return validNameBuilder.ToString();
        }

        private static bool IsValidChar(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;

            if (c >= 'A' && c <= 'Z')
                return true;

            if (c >= '0' && c <= '9')
                return true;

            if (c == '-' || c == '_')
                return true;

            return false;
        }
    }
}