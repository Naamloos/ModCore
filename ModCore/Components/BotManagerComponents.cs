using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Modals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class BotManagerComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;
        private DiscordClient client;

        public BotManagerComponents(DatabaseContextBuilder database, DiscordClient client)
        {
            this.database = database;
            this.client = client;
        }

        [Component("fb", ComponentType.Button)]
        public async Task RespondFeedbackAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            if (Client.CurrentApplication.Owners.All(x => x.Id != e.User.Id))
                return;

            if (ulong.TryParse(context["u"], out _) && ulong.TryParse(context["g"], out _))
            {
                await Client.GetInteractionExtension().RespondWithModalAsync<FeedbackResponseModal>(e.Interaction, "Response to feedback.",
                    new Dictionary<string, string>()
                    {
                        { "g", context["g"] },
                        { "u", context["u"] }
                    });
            }
        }

        [Component("lguild", ComponentType.Button)]
        public async Task LeaveGuildAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            if (Client.CurrentApplication.Owners.All(x => x.Id != e.User.Id))
                return;

            if (ulong.TryParse(context["id"], out ulong id))
            {
                var naughtyGuild = await client.GetGuildAsync(id);
                if (naughtyGuild is not null)
                {
                    await naughtyGuild.LeaveAsync();
                    var response = new DiscordInteractionResponseBuilder()
                    .WithContent("")
                    .AsEphemeral()
                    .AddEmbed(new DiscordEmbedBuilder().WithTitle($"Left naughty guild.")
                        .WithDescription($"{id}: {naughtyGuild.Name}")
                        .WithThumbnail(naughtyGuild.GetIconUrl(ImageFormat.Png))
                        .WithColor(DiscordColor.Red));

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
                }
            }
        }
    }
}
