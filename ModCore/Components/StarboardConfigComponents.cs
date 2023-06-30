using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageGuild)]
    public class StarboardConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public StarboardConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("sb.toggle", ComponentType.Button)]
        public async Task ToggleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Starboard.Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} the starboard module.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("sb.channel", ComponentType.ChannelSelect)]
        public async Task ChangeChannelAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Resolved.Channels.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Starboard.ChannelId = value.Key;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your starboard channel to <#{value.Key}>.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("sb.min", ComponentType.StringSelect)]
        public async Task SetMinimumAsync(ComponentInteractionCreateEventArgs e)
        {
            if (!int.TryParse(e.Interaction.Data.Values[0], out int value))
            {
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ {e.Interaction.Data.Values[0]} is an invalid integer!")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
                return;
            }

            if(value < 1 || value > 10)
            {
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ {value} is out of range! (1-10)")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
                return;
            }

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Starboard.Minimum = value;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your starboard minimum to {value}.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        // regex borrowed:tm: from dsharpplus
        private static Regex EmoteRegex = new Regex(@"^<(?<animated>a)?:(?<name>[a-zA-Z0-9_]+?):(?<id>\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);

        [Component("sb.emoji", ComponentType.Button)]
        public async Task SetEmojiAsync(ComponentInteractionCreateEventArgs e)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                .WithContent("❓ Please send an emoji _(without any other text)_ in this channel within the next 10 seconds to set an emoji..."));

            var interactivity = Client.GetInteractivity();
            var response = await interactivity.WaitForMessageAsync(x => x.ChannelId == e.Channel.Id && x.Author.Id == e.User.Id);

            if(response.TimedOut)
            {
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ Timed out.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
                return;
            }

            try
            {
                await response.Result.DeleteAsync();
            }catch(Exception)
            {
                // meh silent catch
            }

            // Trying to parse emoji, simplified version from what cnext does
            if (!DiscordEmoji.TryFromUnicode(response.Result.Content, out var emoji))
            {
                var match = EmoteRegex.Match(response.Result.Content);
                if (match.Success)
                {
                    DiscordEmoji.TryFromGuildEmote(Client, ulong.Parse(match.Groups["id"].Value), out emoji);
                }
            }

            if(emoji == null)
            {
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ You haven't provided any (valid) emoji!")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
                return;
            }

            if(emoji.Id > 1 && e.Guild.Emojis.All(x => x.Key != emoji.Id))
            {
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ This emoji is not from this Server!")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
                return;
            }

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Starboard.Emoji.EmojiName = emoji.Name;
                settings.Starboard.Emoji.EmojiId = emoji.Id;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set starboard emoji to {emoji}!")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Back to Starboard config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            await using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).Starboard;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("⭐ Starboard Configuration")
                    .WithDescription("Starboard allows members to respond to a message with a specific emoji, sending it to a special starboard channel." +
                        " Starboards can serve as an archive of sorts, listing the community's favorite messages.")
                    .AddField("Enabled", $"{(settings.Enable ? "✅" : "⛔")} This module is currently **{(settings.Enable ? "enabled" : "disabled")}**.")
                    .AddField("Starboard channel", $"<#{settings.ChannelId}>")
                    .AddField("Minimum amount of reactions", $"Currently set to **{settings.Minimum}**.")
                    .AddField("Star emoji", settings.Emoji.GetStringRepresentation());

                var enableId = ExtensionStatics.GenerateIdString("sb.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("sb.toggle", new Dictionary<string, string>() { { "on", "false" } });

                var minimumOptions = new List<DiscordSelectComponentOption>();
                for (int i = 1; i <= 10; i++)
                    minimumOptions.Add(new DiscordSelectComponentOption($"{i}", $"{i}", $"Set Starboard minimum to {i}."));

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.Enable? ButtonStyle.Danger : ButtonStyle.Success, settings.Enable? disableId : enableId, settings.Enable? "Disable Starboard" : "Enable Starboard"),
                        new DiscordButtonComponent(ButtonStyle.Primary, "sb.emoji", "Change Emoji...")
                    })
                    .AddComponents(new DiscordChannelSelectComponent("sb.channel", "Change Channel...", new List<ChannelType>() { ChannelType.Text }))
                    .AddComponents(new DiscordSelectComponent("sb.min", "Set Starboard minimum...", minimumOptions))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
