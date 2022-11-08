using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageGuild)]
    public class NicknameApprovalConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public NicknameApprovalConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("nick.toggle", ComponentType.Button)]
        public async Task ToggleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.NicknameConfirm.Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} the Nickname Approval module.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "nick", "Back to Nickname Approval config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("nick.channel", ComponentType.ChannelSelect)]
        public async Task ChangeChannelAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Resolved.Channels.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.NicknameConfirm.ChannelId = value.Key;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your nickname approval channel to <#{value.Key}>.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "nick", "Back to Nickname Approval config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).NicknameConfirm;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("📝 Nickname Approval Configuration")
                    .WithDescription("Nickname approval allows members to send nickname requests to moderators, who in turn can (dis)approve these nicknames. " +
                        "For this system to work, the member should not have nickname permissions.")
                    .AddField("Enabled", $"{(settings.Enable ? "✅" : "⛔")} This module is currently **{(settings.Enable ? "enabled" : "disabled")}**.")
                    .AddField("Nickname Approval Channel", $"<#{settings.ChannelId}>");

                var enableId = ExtensionStatics.GenerateIdString("nick.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("snick.toggle", new Dictionary<string, string>() { { "on", "false" } });

                var minimumOptions = new List<DiscordSelectComponentOption>();
                for (int i = 1; i <= 10; i++)
                    minimumOptions.Add(new DiscordSelectComponentOption($"{i}", $"{i}", $"Set Starboard minimum to {i}."));

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordButtonComponent(settings.Enable? ButtonStyle.Danger : ButtonStyle.Success, settings.Enable? disableId : enableId, settings.Enable? "Disable Nickname Approval" : "Enable Nickname Approval"))
                    .AddComponents(new DiscordChannelSelectComponent("nick.channel", "Change Channel...", new List<ChannelType>() { ChannelType.Text }))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
