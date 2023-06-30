using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class NoBotFarm
    {
        [AsyncListener(EventType.GuildCreated)]
        public static async Task NoBotFarmsPleaseAsync(GuildCreateEventArgs e, DiscordClient client, Settings settings)
        {
            client.Logger.LogInformation($"New guild joined: {e.Guild.Name}. Starting sus-guild check...");
            int botCount = -1;

            // pre-fetch logging channel
            var channel = await client.GetChannelAsync(settings.SusGuildChannelId);

            if(e.Guild.MemberCount <= 2000)
            {
                var allMembers = await e.Guild.GetAllMembersAsync();
                botCount = allMembers.Count(x => x.IsBot);
            }

            double botRatio = ((double)botCount / (double)e.Guild.MemberCount) * 100d;

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Joined new guild!")
                .WithDescription($"Guild Name: {e.Guild.Name}")
                .AddField("Members", e.Guild.MemberCount.ToString(), true)
                .AddField("Bots", botCount >= 0 ? botCount.ToString() : "LargeGuild", true)
                .AddField("Bot Ratio", botCount >= 0 ? $"{botRatio}%" : "LargeGuild", true);
            if(!string.IsNullOrEmpty(e.Guild.IconHash))
                embed.WithThumbnail(e.Guild.GetIconUrl(ImageFormat.Auto));

            var messageBuilder = new DiscordMessageBuilder();
            if (botRatio > 25)
            {
                var owners = client.CurrentApplication.Owners;
                messageBuilder.WithContent($"This guild has an extraordinarily large bot ratio! Please assess this guild's legitimacy!\n"
                    + $"{string.Join(", ", owners.Select(x => x.Mention))}");
                embed.WithColor(DiscordColor.Red);
                messageBuilder.WithAllowedMentions(owners.Select(x => new UserMention(x)).Cast<IMention>());
            }
            else
            {
                embed.WithColor(DiscordColor.Green);
            }

            messageBuilder.AddEmbed(embed);

            await channel.SendMessageAsync(messageBuilder);
        }
    }
}
