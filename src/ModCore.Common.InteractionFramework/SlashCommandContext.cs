using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework
{
    public class SlashCommandContext
    {
        public InteractionCreate EventData { get; private set; }
        public DiscordRest RestClient { get; private set; }
        public Gateway Gateway { get; private set; }
        public List<ApplicationCommandInteractionDataOption> OptionValues { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        internal SlashCommandContext(InteractionCreate eventData, DiscordRest restClient, Gateway gatewayClient, 
            List<ApplicationCommandInteractionDataOption> optionValues, IServiceProvider serviceProvider) 
        {
            this.EventData = eventData;
            this.RestClient = restClient;
            this.Gateway = gatewayClient;
            this.OptionValues = optionValues;
            this.ServiceProvider = serviceProvider;
        }
    }
}
