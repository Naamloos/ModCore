using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Consumer.Interactions.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ModCore.Services.Consumer.Interactions
{
    public class TestArgumentsCommand : BaseApplicationCommand
    {
        public override string Name => "this_is_a_test";
        public override string Description => "ModCore v3 test command for argument handling.";
        public override Permissions DefaultMemberPermissions => Permissions.Administrator;

        private DiscordRest _rest;

        public TestArgumentsCommand(DiscordRest rest)
        {
            _rest = rest;
        }

        [ApplicationCommandHandler]
        public async Task HandleAsync(Interaction interaction,
            [Description("User to mention")] User targetuser,
            [Description("Channel to mention")] Channel targetchannel,
            [Description("Role to mention")] Role targetrole,
            [Description("Integer to mention")] int targetinteger,
            [Description("Decimal to mention")] double targetdecimal,
            [Description("Text to mention")] string targettext,
            [Description("swag")] Attachment swag,
            [Description("Optional text to mention")] string optionaltext = ""
        )
        {
            var swagIsImage = swag.ContentType.HasValue && swag.ContentType.Value.StartsWith("image/");
            await _rest.CreateInteractionResponseAsync(
                interaction.Id, 
                interaction.Token, 
                InteractionResponseType.ChannelMessageWithSource, 
                new MessageBuilder()
                    .AddContainer(container =>
                    {
                        container.AddText($"Mentioning user: {targetuser.Mention}");
                        container.AddText($"Mentioning channel: {targetchannel.Mention}");
                        container.AddText($"Mentioning role: <@&{targetrole.Id}>");
                        container.AddText($"Number input: {targetinteger}");
                        container.AddText($"Decimal input: {targetdecimal}");
                        container.AddText($"Text input: {targettext}");
                        container.AddText($"Optional input: {(optionaltext.Length > 0 ? optionaltext : "NONE PROVIDED")}");

                        container.AddSection(section =>
                        {
                            section.AddText("Swag attachment");
                        }, swagIsImage? new Thumbnail()
                        {
                            Media = new UnfurledMediaItem()
                            {
                                Url = swag.Url
                            }
                        } : new Button()
                        {
                            Style = ButtonStyle.Link,
                            Label = "View Swag",
                            Url = swag.Url,
                        });
                    })
                    .WithFlags(MessageFlags.Ephemeral)
                    .BuildInteractionResponse());
        }
    }
}
