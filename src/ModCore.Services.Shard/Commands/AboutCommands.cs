using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Commands
{
    public class AboutCommands : BaseCommandHandler
    {
        [SlashCommand("Shows information about this bot.", dm_permission: true)]
        public async Task About(SlashCommandContext context)
        {
            await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token, 
                InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                {
                    Content = "hewwo :3"
                });
        }

        [SlashCommand("Just a test(icle)")]
        public class Test
        {
            [SlashCommand("mention")]
            public async Task Mention(SlashCommandContext context,
                [Option("Test parameter", ApplicationCommandOptionType.User)]Optional<Snowflake> test)
            {
                string mention = "You didn't supply a value!";
                if(test.HasValue)
                {
                    mention = $"hello, {context.EventData.Data.Value.Resolved.Value.Users.Value[test.Value.ToString()].Username}! <@{test}>";
                }

                await context.RestClient.CreateInteractionResponseAsync(context.EventData.Id, context.EventData.Token,
                    InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                    {
                        Content = mention
                    });
            }
        }
    }
}
