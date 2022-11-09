using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Extensions.Abstractions;

namespace ModCore.Modals
{
    [Modal("massban")]
    public class MassBanModal : IModal
    {
        [ModalField("Member IDs, separated by comma", "members", placeholder: "Enter IDs here...", style: TextInputStyle.Paragraph)]
        public string Ids { get; set; }

        [ModalField("Reason to ban?", "reason", placeholder: "Reason...", style: TextInputStyle.Paragraph)]
        public string Reason { get; set; }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);
            
            if(!member.Permissions.HasFlag(Permissions.BanMembers))
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⚠️ You shouldn't be able to use this modal, you do not have the right permissions!"));
                return;
            }

            if(string.IsNullOrWhiteSpace(Ids))
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⚠️ No IDs given!"));
                return;
            }

            var splitIds = Ids.Replace(" ", "").Split(',');

            var banned = new List<ulong>();
            var failed = new List<string>();
            foreach(var id in splitIds)
            {
                if(ulong.TryParse(id, out ulong parsed))
                {
                    try
                    {
                        await interaction.Guild.BanMemberAsync(parsed, 7, string.IsNullOrWhiteSpace(Reason) ? null : Reason);
                        banned.Add(parsed);
                        continue;
                    }
                    catch (Exception) { }
                }

                failed.Add(id);
            }

            var response = $"🚓 Banned {banned.Count} users.";
            if (failed.Count > 0)
                response += $"\nFailed to ban the following IDs: {string.Join(", ", failed)}";

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(response).AsEphemeral());
        }
    }
}
