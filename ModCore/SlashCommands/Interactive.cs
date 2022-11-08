using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using ModCore.Extensions;
using ModCore.Modals;
using ModCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    public class Interactive : ApplicationCommandModule
    {
        // TODO these commands need to be migrated to use the timer system instead.
        [SlashCommand("poll", "Starts a poll in this channel.")]
        public async Task PollAsync(InteractionContext ctx)
            => await ctx.Client.GetInteractionExtension().RespondWithModalAsync<PollModal>(ctx.Interaction, "Create poll...");

        [SlashCommand("raffle", "Starts a raffle / giveaway.")]
        public async Task RaffleAsync(InteractionContext ctx, 
            [Option("prize", "Prize you are giving away.")]string prize, 
            [Option("timespan", "How long to hold the giveaway for.")]string timespan)
        {
            await ctx.DeferAsync();
            var trophy = DiscordEmoji.FromUnicode("🏆");
            var bomb = DiscordEmoji.FromUnicode("💣");

            var button = new DiscordButtonComponent(ButtonStyle.Success, "join", "Join raffle!", emoji: new DiscordComponentEmoji(trophy));
            var leave = new DiscordButtonComponent(ButtonStyle.Danger, "leave", "Leave raffle", emoji: new DiscordComponentEmoji(bomb));
            var (time, _) = Dates.ParseTime(timespan);

            var giveaway = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Hey! {ctx.User.Mention} is giving away `{prize.Replace('`', '\'')}`! Winner to be announced <t:{DateTimeOffset.Now.Add(time).ToUnixTimeSeconds()}:R>")
                .AddComponents(button, leave));

            List<DiscordUser> members = new List<DiscordUser>();

            Task collectorTask(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs e)
            {
                _ = Task.Run(async () =>
                {
                    if(e.Message.Id == giveaway.Id && e.User.Id != ctx.User.Id)
                    {
                        if(e.Interaction.Data.CustomId == "join" && !members.Any(x => x.Id == e.User.Id))
                        {
                            members.Add(e.User);
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("✅ You joined the raffle!").AsEphemeral());
                        }
                        else if(e.Interaction.Data.CustomId == "leave")
                        {
                            members.RemoveAll(x => x.Id == e.User.Id);
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("✅ You left the raffle.").AsEphemeral());
                        }
                        else
                        {
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.Pong);
                        }
                    }
                });

                return Task.CompletedTask;
            }

            ctx.Client.ComponentInteractionCreated += collectorTask;
            await Task.Delay(time);
            ctx.Client.ComponentInteractionCreated -= collectorTask;

            if(members.Count < 1)
            {
                await ctx.EditFollowupAsync(giveaway.Id, new DiscordWebhookBuilder().WithContent($"😢 Nobody joined {ctx.User.Mention}'s raffle, so nobody won `{prize.Replace('`', '\'')}`..."));
                return;
            }

            var winnerindex = new Random().Next(0, members.Count() - 1);
            var winner = members[winnerindex];

            var tada = DiscordEmoji.FromUnicode("🎉");
            await ctx.EditFollowupAsync(giveaway.Id, new DiscordWebhookBuilder().WithContent($"{tada}{trophy} " +
                $"{winner.Mention}, you won `{prize.Replace('`', '\'')}`! Contact {ctx.User.Mention} for your price! " +
                $"{trophy}{tada}"));
        }
    }
}
