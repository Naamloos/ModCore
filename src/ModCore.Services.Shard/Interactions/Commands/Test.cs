using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Shard.Interactions.Attributes;
using ModCore.Services.Shard.Interactions.InteractionTypes;

namespace ModCore.Services.Shard.Interactions.Commands
{
    [Command("test", "test")]
    public class Test : BaseCommand
    {
        private DiscordRest _rest;

        public Test(DiscordRest rest)
        {
            _rest = rest;
        }

        [Subcommand("subcommand", "subcommand on test")]
        public async Task SubCommandAsync(InteractionCreate data, 
            [Parameter("Some test param")] string param_value,
            [Parameter("Goof")] Optional<double> some_other_param)
        {
            await _rest.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
            {
                Content = "Test response sub",
                Flags = MessageFlags.Ephemeral
            });
        }

        [SubcommandGroup("subcommandgroup", "subcommand group under test")]
        public class Sub : BaseSubcommandGroup<Test>
        {
            public Sub(Test parent) : base(parent){}

            [Subcommand("subcommand", "subcommand on subcommand group under test")]
            public async Task SubCommandAsync(InteractionCreate data)
            {
                await Parent._rest.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
                {
                    Content = "Test response sub-sub",
                    Flags = MessageFlags.Ephemeral
                });
            }
        }
    }
}
