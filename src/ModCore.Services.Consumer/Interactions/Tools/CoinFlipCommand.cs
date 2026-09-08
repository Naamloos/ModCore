using Microsoft.Extensions.Configuration;
using ModCore.Common.Configuration;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Language;
using ModCore.Services.Consumer.Interactions.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Consumer.Interactions.Tools
{
    public class CoinFlipCommand : BaseApplicationCommand
    {
        public override string Name => "coinflip";

        public override string Description => "Flip a coin";
        
        private readonly DiscordRest _rest;
        private readonly IModCoreLocalizerFactory _localizerFactory;
        private readonly IConfiguration _configuration;

        public CoinFlipCommand(DiscordRest rest, IModCoreLocalizerFactory localizerFactory, IConfiguration configuration) {
            _rest = rest;
            _localizerFactory = localizerFactory;
            _configuration = configuration;
        }


        [ApplicationCommandHandler]
        public async Task ExecuteAsync(Interaction interaction)
        {
            var t = _localizerFactory.Get(interaction.Locale);
            var appId = new Snowflake(_configuration.GetValue<ulong>(ConfigurationHelper.GetConfigKeyString(ConfigKey.ApplicationId)));

            var messageBuilder = new MessageBuilder()
                .WithContent(t["coinFlipStart"])
                .WithFlags(MessageFlags.Ephemeral);

            await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource,
                messageBuilder.BuildInteractionResponse());
            await Task.Delay(1000);

            messageBuilder.WithContent($"​ㅤ\n✊🪙");
            await _rest.EditOriginalInteractionResponseAsync(appId, interaction.Token,
                messageBuilder.Build());
            await Task.Delay(200);

            messageBuilder.WithContent($"​ㅤㅤ🪙ㅤㅤ\n☝️✨");
            await _rest.EditOriginalInteractionResponseAsync(appId, interaction.Token,
                messageBuilder.Build());
            await Task.Delay(200);

            messageBuilder.WithContent($"​ㅤㅤ✨🪙ㅤ\n☝️✨");
            await _rest.EditOriginalInteractionResponseAsync(appId, interaction.Token,
                messageBuilder.Build());
            await Task.Delay(200);

            messageBuilder.WithContent($"ㅤㅤ✨✨\n​☝️✨ㅤㅤ🪙");
            await _rest.EditOriginalInteractionResponseAsync(appId, interaction.Token,
                messageBuilder.Build());
            await Task.Delay(1000);

            var rng = Random.Shared.Next(0, 2);
            messageBuilder.WithContent(t["coinFlipFinish", new { result = rng == 0 }]);
            await _rest.EditOriginalInteractionResponseAsync(appId, interaction.Token,
                messageBuilder.Build());
        }
    }
}
