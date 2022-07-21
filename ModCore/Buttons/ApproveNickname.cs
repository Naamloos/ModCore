using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions.Buttons.Attributes;
using ModCore.Extensions.Buttons.Interfaces;
using System;
using System.Threading.Tasks;

namespace ModCore.Buttons
{
    [Button("b")]
    public class ApproveNickname : IButton
    {
        [ButtonField("n")]
        public string Name { get; set; }

        [ButtonField("u")]
        public string UserId { get; set; }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            if (ulong.TryParse(UserId, out var id))
            {
                var guild = interaction.Guild;
                var member = await guild.GetMemberAsync(interaction.User.Id);

                if (!member.Permissions.HasPermission(Permissions.ManageNicknames))
                    return;

                var targetMember = await guild.GetMemberAsync(id);

                await targetMember.ModifyAsync(x => x.Nickname = Name);

                var embed = new DiscordEmbedBuilder()
                    .WithAuthor(targetMember.Username + "#" + targetMember.Discriminator, iconUrl: targetMember.GetAvatarUrl(ImageFormat.Png))
                    .WithDescription($"Nickname approved")
                    .AddField("New nickname", Name)
                    .AddField("Old nickname", targetMember.DisplayName)
                    .AddField("Responsible moderator", member.Username + "#" + member.Discriminator)
                    .WithColor(DiscordColor.Green);

                var msg = new DiscordMessageBuilder()
                    .AddEmbed(embed);

                await message.ModifyAsync(msg);

                try
                {
                    await targetMember.SendMessageAsync($"✅ Your request to change your nickname in {interaction.Guild.Name} to {Name} has been approved.");
                }
                catch (Exception) { }
            }
        }
    }
}
