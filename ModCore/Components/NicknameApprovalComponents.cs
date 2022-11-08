using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.VisualBasic;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageNicknames)]
    public class NicknameApprovalComponents : BaseComponentModule
    {
        [Component("nick.yes", ComponentType.Button)]
        public async Task ApproveNicknameAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            string nickname = context["n"];
            if (ulong.TryParse(context["u"], out ulong user_id))
            {
                var guild = e.Guild;

                var targetMember = await guild.GetMemberAsync(user_id);

                await targetMember.ModifyAsync(x => x.Nickname = nickname);

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor(targetMember.Username + "#" + targetMember.Discriminator, iconUrl: targetMember.GetAvatarUrl(ImageFormat.Png))
                    .WithDescription($"Nickname approved")
                    .AddField("New nickname", nickname)
                    .AddField("Old nickname", targetMember.DisplayName)
                    .AddField("Responsible moderator", e.User.Username + "#" + e.User.Discriminator)
                    .WithColor(DiscordColor.Green);

                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(embed));

                try
                {
                    await targetMember.SendMessageAsync($"✅ Your request to change your nickname in {e.Interaction.Guild.Name} to {nickname} has been approved.");
                }
                catch (Exception) { }
            }
        }

        [Component("nick.no", ComponentType.Button)]
        public async Task DisapproveNicknameAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            string nickname = context["n"];
            if (ulong.TryParse(context["u"], out ulong user_id))
            {
                var guild = e.Guild;

                var targetMember = await guild.GetMemberAsync(user_id);

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor(targetMember.Username + "#" + targetMember.Discriminator, iconUrl: targetMember.GetAvatarUrl(ImageFormat.Png))
                    .WithDescription($"Nickname Denied")
                    .AddField("New nickname", nickname)
                    .AddField("Old nickname", targetMember.DisplayName)
                    .AddField("Responsible moderator", e.User.Username + "#" + e.User.Discriminator)
                    .WithColor(DiscordColor.Red);

                var msg = new DiscordMessageBuilder()
                    .AddEmbed(embed);

                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(embed));

                try
                {
                    await targetMember.SendMessageAsync($"⛔ Your request to change your nickname in {e.Interaction.Guild.Name} to {nickname} has been disapproved.");
                }
                catch (Exception) { }
            }
        }
    }
}
