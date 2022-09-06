using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions;
using ModCore.Modals;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    [SlashCommandGroup("config", "ModCore Configuration Commands")]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    public class Config : ApplicationCommandModule
    {
        [SlashCommandGroup("logging", "Configuration for ModCore's logging module.")]
        public class Logger
        {
            public DatabaseContext Database { private get; set; }

            [SlashCommand("togglejoins", "Toggles join logs.")]
            public async Task ToggleJoinlogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.JoinLog_Enable = !config.Logging.JoinLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Join and leave logging was set to `{config.Logging.JoinLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("togglemessages", "Toggles message edit and delete logging for this server.")]
            public async Task ToggleEditLogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.EditLog_Enable = !config.Logging.EditLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Message edit / delete logging was set to `{config.Logging.EditLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("togglenicknames", "Toggles nickname update logging for this server.")]
            public async Task ToggleNicknameLogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.NickameLog_Enable = !config.Logging.NickameLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Nickname change logging was set to `{config.Logging.NickameLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("toggleinvites", "Toggles invite creation logging for this server.")]
            public async Task ToggleInviteLogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.InviteLog_Enable = !config.Logging.InviteLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Invite creation logging was set to `{config.Logging.InviteLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("toggleavatars", "Toggles avatar update logging for this server.")]
            public async Task ToggleAvatarLogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.AvatarLog_Enable = !config.Logging.AvatarLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Avatar update logging was set to `{config.Logging.AvatarLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("togglemoderation", "Toggles moderative action logging for this server.")]
            public async Task ToggleModLogAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.ModLog_Enable = !config.Logging.ModLog_Enable;
                    await ctx.CreateResponseAsync($"✅ Moderative action logging was set to `{config.Logging.ModLog_Enable}` for this server.", true);
                });
            }

            [SlashCommand("setchannel", "Sets logging channel for this server.")]
            public async Task SetLoggingChannelAsync(InteractionContext ctx, [Option("Channel", "Channel to send logs to.")]DiscordChannel channel)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Logging.ChannelId = channel.Id;
                    await ctx.CreateResponseAsync($"✅ Logging channel was set to {channel.Mention} for this server.", true);
                });
            }
        }

        [SlashCommandGroup("filters", "Configuration for ModCore's chat filter module.")]
        [SlashCommandPermissions(Permissions.ManageGuild)]
        public class Filters
        {
            // TODO phishing filters (CHECK GH ISSUES)
            [SlashCommand("toggleinvites", "Toggles invite filters.")]
            public async Task ToggleInvitesAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Linkfilter.BlockInviteLinks = !config.Linkfilter.BlockInviteLinks;
                    await ctx.CreateResponseAsync($"✅ Invite filter was set to {config.Linkfilter.BlockInviteLinks} for this server.", true);
                });
            }
        }

        [SlashCommandGroup("selfrole", "Configuration for ModCore's self-assignable roles module.")]
        [SlashCommandPermissions(Permissions.ManageGuild)]
        public class SelfRoles
        {
            [SlashCommand("add", "Adds role to self-assignable roles.")]
            public async Task AddAsync(InteractionContext ctx, [Option("role", "Role to add to self-assignable roles")] DiscordRole role)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    if(config.SelfRoles.Any(x => x == role.Id))
                    {
                        await ctx.CreateResponseAsync($"❌ The {role.Mention} role is already registered as a self-assignable role.", true);
                        return;
                    }

                    config.SelfRoles.Add(role.Id);
                    await ctx.CreateResponseAsync($"✅ {role.Mention} was added as a self-assignable role.", true);
                });
            }

            [SlashCommand("remove", "Removes role from self-assignable roles.")]
            public async Task RemoveAsync(InteractionContext ctx, [Option("role", "Role to remove from self-assignable roles")] DiscordRole role)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    if (!config.SelfRoles.Any(x => x == role.Id))
                    {
                        await ctx.CreateResponseAsync($"❌ The {role.Mention} role is not registered as a self-assignable role.", true);
                        return;
                    }

                    config.SelfRoles.Remove(role.Id);
                    await ctx.CreateResponseAsync($"✅ {role.Mention} was removed from self-assignable roles.", true);
                });
            }

            [SlashCommand("list", "Lists self-assignable roles.")]
            public async Task ListAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle("Self-assignable roles")
                        .WithDescription(string.Join(" ", config.SelfRoles.Select(x => $"<@!{x}>")));

                    await ctx.CreateResponseAsync(embed, true);
                });
            }
        }

        [SlashCommandGroup("starboard", "Configuration for ModCore's starboard module.")]
        [SlashCommandPermissions(Permissions.ManageGuild)]
        public class Starboard
        {
            [SlashCommand("setchannel", "Sets starboard channel.")]
            public async Task SetChannelAsync(InteractionContext ctx, 
                [Option("channel", "Channel to post starboard messages to. Set to null to disable starboard.")]DiscordChannel channel = null)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Starboard.Enable = channel != null;
                    config.Starboard.ChannelId = channel != null ? channel.Id : 0;
                    if (channel != null)
                        await ctx.CreateResponseAsync($"✅ Starboard channel was set to {channel.Mention}.", true);
                    else
                        await ctx.CreateResponseAsync($"✅ Starboard was disabled for this server.", true);
                });
            }

            [SlashCommand("setemoji", "Sets starboard emoji.")]
            public async Task SetEmojiAsync(InteractionContext ctx, [Option("emoji", "Emoji to replace stars with.")]DiscordEmoji emoji)
            {
                if(emoji.Id != 0 && !ctx.Guild.Emojis.Any(x => x.Key == emoji.Id))
                {
                    await ctx.CreateResponseAsync($"❌ This emoji can not be used for this guild's starboard! It does not belong to this guild.", true);
                    return;
                }

                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Starboard.Emoji = new GuildEmoji { EmojiId = emoji.Id, EmojiName = emoji.Name };
                    await ctx.CreateResponseAsync($"✅ The starboard now uses the {emoji.ToString()} emoji.", true);
                });
            }

            [SlashCommand("setminimum", "Sets starboard minimum.")]
            public async Task SetMinimumAsync(InteractionContext ctx, [Option("minimum", "Minimum amount of stars required to show up on StarBoard.")]long minimum)
            {
                if(minimum < 1 && minimum > 10)
                {
                    await ctx.CreateResponseAsync($"❌ Starboard minimum has to be between 1 and 10!", true);
                    return;
                }

                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Starboard.Minimum = (int)minimum;
                    await ctx.CreateResponseAsync($"✅ The starboard minimum was set to {minimum}.", true);
                });
            }
        }

        [SlashCommandGroup("levels", "Configuration for ModCore's levelup system.")]
        public class Levels
        {
            [SlashCommand("toggle", "Toggles levelup system for this server.")]
            public async Task ToggleAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Levels.Enabled= !config.Levels.Enabled;
                    await ctx.CreateResponseAsync($"✅ Level system was set to `{config.Levels.Enabled}` for this server.", true);
                });
            }

            [SlashCommand("togglemessages", "Toggles levelup notification messages for this server.")]
            public async Task PostMessagesAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Levels.MessagesEnabled = !config.Levels.MessagesEnabled;
                    await ctx.CreateResponseAsync($"✅ Level system notifications were set to `{config.Levels.MessagesEnabled}` for this server.", true);
                });
            }

            [SlashCommand("redirect-to", "Sets level notification redirect channel for this server.")]
            public async Task RedirectChannel(InteractionContext ctx, 
                [Option("channel", "Channel to redirect notifications to. Leave empty to disable redirecting messages.")]DiscordChannel channel = null)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Levels.RedirectMessages = channel != null;
                    config.Levels.ChannelId = channel != null ? channel.Id : 0;
                    if (channel != null)
                        await ctx.CreateResponseAsync($"✅ Level updates are now redirected to {channel.Mention}.", true);
                    else
                        await ctx.CreateResponseAsync("✅ Level updates no longer get redirected.", true);
                });
            }
        }

        [SlashCommandGroup("set", "Commands that couldn't be grouped otherwise. Blame Discord.")]
        public class Other
        {
            [SlashCommand("nicknamerequests", "Enabled nickname requests to a specific channel.")]
            public async Task NicknameRequests(InteractionContext ctx,
                [Option("channel", "Channel where nickname requests are sent. Leave empty to disable nickname requests.")]
            DiscordChannel channel = null)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.RequireNicknameChangeConfirmation = channel != null;
                    config.NicknameChangeConfirmationChannel = channel != null ? channel.Id : 0;
                    if (channel != null)
                        await ctx.CreateResponseAsync($"✅ Nickname requests are now enabled, and are sent to {channel.Mention}.", true);
                    else
                        await ctx.CreateResponseAsync("✅ Nickname requests were disabled.", true);
                });
            }

            [SlashCommand("welcomemessage", "Sets welcome message for this server.")]
            public async Task SetWelcomeAsync(InteractionContext ctx,
                [Option("channel", "Channel where welcome messages are sent. Leave empty to disable welcome messages.")]
                DiscordChannel channel = null)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.Welcome.Enable = channel != null;
                    config.Welcome.ChannelId = channel != null ? channel.Id : 0;
                    if (channel != null)
                    {
                        await ctx.Client.GetModalExtension().RespondWithModalAsync<WelcomeMessageModal>(ctx.Interaction, "Set welcome message");
                    }
                    else
                        await ctx.CreateResponseAsync("✅ Welcome messages were disabled.", true);
                });
            }


            [SlashCommand("userstates", "Restores a user's nickname, roles and overrides when they rejoin.")]
            [SlashCommandPermissions(Permissions.ManageGuild)]
            public async Task ToggleUserStatesAsync(InteractionContext ctx)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.RoleState.Enable = !config.RoleState.Enable;
                    await ctx.CreateResponseAsync($"✅ User states was set to {config.RoleState.Enable} for this server.", true);
                });
            }

            [SlashCommand("joinrole", "Sets join role for this server.")]
            [SlashCommandPermissions(Permissions.ManageGuild)]
            public async Task SetJoinRoleAsync(InteractionContext ctx,
                [Option("Role", "Role given on join. Leave empty to remove.")] DiscordRole role = null)
            {
                await ctx.WithGuildSettingsAsync(async config =>
                {
                    config.AutoRole.Enable = role != null;
                    config.AutoRole.RoleId = role != null ? role.Id : 0;
                    if (role != null)
                        await ctx.CreateResponseAsync($"✅ New members now receive the {role.Mention} role when joining this server.", true);
                    else
                        await ctx.CreateResponseAsync("✅ Join role has been disabled.", true);
                });
            }
        }
    }
}
