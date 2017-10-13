﻿using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using DSharpPlus.Entities;
using System.Text;
using DSharpPlus;

namespace ModCore.Commands
{
    [Group("info"), Aliases("i"), Description("informative commands")]
    public class Info
    {
        public SharedData Shared { get; }
        public InteractivityExtension Interactivity { get; }

        public Info(SharedData shared, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Interactivity = interactive;
        }

        [Command("user"), Aliases("u")]
        public async Task UserInfoAsync(CommandContext ctx, DiscordMember usr)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.MidnightBlue)
                .WithTitle($"@{usr.Username}#{usr.Discriminator} - ID: {usr.Id}");

            if (usr.IsBot) embed.Title += " __[BOT]__ ";
            if (usr.IsOwner) embed.Title += " __[OWNER]__ ";

            embed.Description =
                $"Registered on     : {usr.CreationTimestamp.DateTime.ToString()}\n" +
                $"Joined Guild on  : {usr.JoinedAt.DateTime.ToString()}";

            var roles = new StringBuilder();
            foreach (var r in usr.Roles) roles.Append($"[{r.Name}] ");
            if (roles.Length == 0) roles.Append("*None*");
            embed.AddField("Roles", roles.ToString());

            var permsobj = usr.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = "**[!] User can't see this channel!**\n" + perms;
            if (perms == String.Empty) perms = "*None*";
            embed.AddField("Permissions", perms);

            embed.WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}");

            await ctx.RespondAsync("", false, embed: embed);
        }

        [Command("guild"), Aliases("g")]
        public async Task GuildInfoAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("The following embed might flood this channel. Do you want to proceed?");
            var m = await Interactivity.WaitForMessageAsync(x => x.Content.ToLower() == "yes" || x.Content.ToLower() == "no");
            if (m?.Message?.Content == "yes")
            {
                #region yes
                var g = ctx.Guild;
                var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.PhthaloBlue)
                    .WithTitle($"{g.Name} ID: ({g.Id})")
                    .WithDescription($"Created on: {g.CreationTimestamp.DateTime.ToString()}\n" +
                    $"Member count: {g.MemberCount}" +
                    $"Joined at: {g.JoinedAt.DateTime.ToString()}");
                if (!string.IsNullOrEmpty(g.IconHash))
                    embed.WithThumbnailUrl(g.IconUrl);
                embed.WithAuthor($"Owner: {g.Owner.Username}#{g.Owner.Discriminator}", icon_url: string.IsNullOrEmpty(g.Owner.AvatarHash) ? null : g.Owner.AvatarUrl);
                var cs = new StringBuilder();
                #region channel list string builder
                foreach (var c in g.Channels)
                {
                    switch (c.Type)
                    {
                        case ChannelType.Text:
                            cs.Append($"[`#{c.Name} (💬)`]");
                            break;
                        case ChannelType.Voice:
                            cs.Append($"`[{c.Name} (🔈)]`");
                            break;
                        case ChannelType.Category:
                            cs.Append($"`[{c.Name.ToUpper()} (📁)]`");
                            break;
                        default:
                            cs.Append($"`[{c.Name} (❓)]`");
                            break;
                    }
                }
                #endregion
                embed.AddField("Channels", cs.ToString());

                var rs = new StringBuilder();
                #region role list string builder
                foreach (var r in g.Roles)
                {
                    rs.Append($"[`{r.Name}`] ");
                }
                #endregion
                embed.AddField("Roles", rs.ToString());

                var es = new StringBuilder();
                #region emoji list string builder
                foreach (var e in g.Emojis)
                {
                    es.Append($"[`{e.Name}`] ");
                }
                #endregion
                embed.AddField("Emotes", es.ToString());

                embed.AddField("Voice", $"AFK Channel: {(g.AfkChannel != null ? $"#{g.AfkChannel.Name}" : "None.")}\n" +
                    $"AFK Timeout: {g.AfkTimeout}\n" +
                    $"Region: {g.RegionId}");

                embed.AddField("Misc", $"Large: {(g.IsLarge ? "yes" : "no")}.\n" +
                    $"Default Notifications: {g.DefaultMessageNotifications}.\n" +
                    $"Explicit content filter: {g.ExplicitContentFilter}.\n" +
                    $"MFA Level: {g.MfaLevel}.\n" +
                    $"Verification Level: {g.VerificationLevel}");

                await ctx.RespondAsync("", false, embed: embed);
                #endregion
            }
            else
            {
                #region no or timeout
                await ctx.RespondAsync("Okay, I'm not sending the embed.");
                #endregion
            }
        }
    }
}