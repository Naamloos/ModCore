using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Helpers;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Language;
using ModCore.Common.Utils;
using ModCore.Services.Consumer.Interactions.Framework;
using System.ComponentModel;

namespace ModCore.Services.Consumer.Interactions.Moderation
{
    public class BanCommand : BaseApplicationCommand
    {
        public override string Name => "ban";
        public override string Description => "Bans a specific user from the server";
        public override Permissions DefaultMemberPermissions => Permissions.BanMembers;

        private readonly ILogger _logger;
        private readonly CacheService _cache;
        private readonly DiscordRest _rest;
        private readonly DatabaseContext _database;
        private readonly IModCoreLocalizerFactory _localizerFactory;

        public BanCommand(CacheService cache, DiscordRest rest, DatabaseContext database, ILogger<BanCommand> logger, IModCoreLocalizerFactory localizerFactory)
        {
            _logger = logger;
            _cache = cache;
            _rest = rest;
            _database = database;
            _localizerFactory = localizerFactory;
        }

        [ApplicationCommandHandler]
        public async Task HandleAsync(
            Interaction interaction,
            [Description("Target user to ban")] User target,
            [Description("Reason the user got banned")] string reason = "No reason given.",
            [Description("Whether to notify the user by DM")] bool notify = true
        )
        {
            var t = _localizerFactory.Get(interaction.GuildLocale);

            Guild? guild = null;

            if (!interaction.GuildId.HasValue)
            {
                return;
            }

            var guildId = interaction.GuildId.Value;

            var cacheResult = _cache.TryGet<Guild, Snowflake>(guildId);
            if (cacheResult.Success)
            {
                guild = cacheResult.Value;
            }
            else
            {
                var restResult = await _rest.GetGuildAsync(guildId);
                if (restResult.Success)
                {
                    guild = restResult.Value;
                    await _cache.UpdateAsync(guild.Id, guild);
                }
            }

            if (guild is null)
            {
                // Failure, notify user
                var failureResponse = new MessageBuilder()
                    .AddContainer(container =>
                    {
                        container.AddText(t["banFailure", new { user = target }]);
                    }).WithFlags(MessageFlags.Ephemeral).BuildInteractionResponse();
                return;
            }

            var dmChannel = await _rest.CreateDMChannelAsync(target.Id);

            var sentDM = false;
            if (dmChannel.Success)
            {
                var dmMessage = new MessageBuilder()
                    .AddContainer(container =>
                    {
                        container.AddText(t["banNotifyMessage", new { guild = guild.Name, reason = reason.InCodeBlock() }]);
                    }).Build();
                var dm = await _rest.CreateMessageAsync(dmChannel.Value.Id, dmMessage);
                sentDM = dm.Success;
            }

            // TODO re-enable once we launch?
            //var ban = await _rest.CreateGuildBanAsync(guildId, target.Id);

            if (true)
            {
                var successResponse = new MessageBuilder().AddContainer(container =>
                {
                    container.AddText(t["banSuccess", new { user = target.Mention, reason = reason.InCodeBlock(), sentDM }]);
                }).WithFlags(MessageFlags.Ephemeral).BuildInteractionResponse();
                await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource,
                    successResponse);

                var infractionHelper = new InfractionHelper(_database, target.Id, guildId);
                await infractionHelper.CreateInfractionAsync(InfractionType.Ban, target.Id, reason = reason.InCodeBlock(), sentDM);
            }
            else
            {
                var failureAlreadyDMed = new MessageBuilder().AddContainer(container =>
                {
                    container.AddText(t["banFailureDmAlreadySent", new { user = target.Mention, reason = reason.InCodeBlock() }]);
                }).WithFlags(MessageFlags.Ephemeral).BuildInteractionResponse();

                await _rest.CreateInteractionResponseAsync(interaction.Id, interaction.Token, InteractionResponseType.ChannelMessageWithSource,
                    failureAlreadyDMed);
            }
        }
    }
}
