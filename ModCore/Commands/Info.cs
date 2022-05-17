using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("info"), Aliases("i"), Description("Information commands"), CheckDisable]
    public class Info : BaseCommandModule
	{
        public SharedData Shared { get; }
        public InteractivityExtension Interactivity { get; }

        public Info(SharedData shared, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Interactivity = interactive;
        }

		[GroupCommand]
        public async Task ExecuteGroupAsync(CommandContext context)
        {
            var prefix = context.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var commandstring = "help info";
            var commandobject = context.CommandsNext.FindCommand(commandstring, out string args);
            var fakecontext = context.CommandsNext.CreateFakeContext(context.Member, context.Channel, commandstring, prefix, commandobject, args);
            await context.CommandsNext.ExecuteCommandAsync(fakecontext);
        }

        [Command("user"), Aliases("u"), Description("Returns information about a specific user."), CheckDisable]
        public async Task UserInfoAsync(CommandContext context, [Description("Member to get information about")]DiscordMember member)
        {

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"@{member.Username}#{member.Discriminator} - ID: {member.Id}");

            if (member.IsBot) 
                embed.Title += " __[BOT]__ ";

            if (member.IsOwner) 
                embed.Title += " __[OWNER]__ ";

            embed.Description =
                $"Registered on     : {member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}\n" +
                $"Joined Guild on  : {member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture)}";

            var roles = new StringBuilder();

            foreach (var r in member.Roles) 
                roles.Append($"[{r.Name}] ");

            if (roles.Length == 0) 
                roles.Append("*None*");

            embed.AddField("Roles", roles.ToString());

            var permissionsEnum = member.PermissionsIn(context.Channel);
            var permissions = permissionsEnum.ToPermissionString();
            if (((permissionsEnum & Permissions.Administrator) | (permissionsEnum & Permissions.AccessChannels)) == 0)
                permissions = "**[!] User can't see this channel!**\n" + permissions;

            if (permissions == String.Empty) 
                permissions = "*None*";

            embed.AddField("Permissions", permissions);

            embed.WithFooter($"{context.Guild.Name} / #{context.Channel.Name} / {DateTime.Now}");

            embed.WithThumbnail(member.GetGuildAvatarUrl(ImageFormat.Auto));

            await context.ElevatedRespondAsync(embed: embed);
        }

        [Command("guild"), Aliases("g"), Description("Returns information about this guild."), CheckDisable]
        public async Task GuildInfoAsync(CommandContext context)
        {
            await context.SafeRespondUnformattedAsync("The following embed might flood this channel. Do you want to proceed?");
            var message = await Interactivity.WaitForMessageAsync(x => x.Content.ToLower() == "yes" || x.Content.ToLower() == "no");
            if (message.Result?.Content.ToLowerInvariant() == "yes")
            {
                #region yes
                var guild = context.Guild;

                var embed = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#089FDF"))
                    .WithTitle($"{guild.Name} ID: ({guild.Id})")
                    .WithDescription($"Created on: {guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}\n" +
                    $"Member count: {guild.MemberCount}\n" +
                    $"Joined at: {guild.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture)}");

                if (!string.IsNullOrEmpty(guild.IconHash))
                    embed.WithThumbnail(guild.IconUrl);

                embed.WithAuthor($"Owner: {guild.Owner.Username}#{guild.Owner.Discriminator}", 
                    iconUrl: string.IsNullOrEmpty(guild.Owner.AvatarHash) ? null : guild.Owner.AvatarUrl);
                var channelstring = new StringBuilder();
                #region channel list string builder
                foreach (var channel in guild.Channels)
                {
                    switch (channel.Value.Type)
                    {
                        case ChannelType.Text:
                            channelstring.Append($"[`#{channel.Value.Name} (💬)`]");
                            break;
                        case ChannelType.Voice:
                            channelstring.Append($"`[{channel.Value.Name} (🔈)]`");
                            break;
                        case ChannelType.Category:
                            channelstring.Append($"`[{channel.Value.Name.ToUpper()} (📁)]`");
                            break;
                        default:
                            channelstring.Append($"`[{channel.Value.Name} (❓)]`");
                            break;
                    }
                }
                #endregion
                embed.AddField("Channels", channelstring.ToString());

                var rolestring = new StringBuilder();
                #region role list string builder
                foreach (var role in guild.Roles)
                {
                    rolestring.Append($"[`{role.Value.Name}`] ");
                }
                #endregion
                embed.AddField("Roles", rolestring.ToString());

                embed.AddField("Misc", $"Large: {(guild.IsLarge ? "yes" : "no")}.\n" +
                    $"Default Notifications: {guild.DefaultMessageNotifications}.\n" +
                    $"Explicit content filter: {guild.ExplicitContentFilter}.\n" +
                    $"MFA Level: {guild.MfaLevel}.\n" +
                    $"Verification Level: {guild.VerificationLevel}");

                embed.WithThumbnail(guild.GetIconUrl(ImageFormat.Auto));

                await context.ElevatedRespondAsync(embed: embed);
                #endregion
            }
            else
            {
                #region no or timeout
                await context.SafeRespondUnformattedAsync("Okay, I'm not sending the embed.");
                #endregion
            }
        }

        [Command("role"), Aliases("r"), Description("Returns information about a specific role."), CheckDisable]
        public async Task RoleInfoAsync(CommandContext context, [Description("Role to get information about")]DiscordRole role)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle($"{role.Name} ID: ({role.Id})")
                .WithDescription($"Created at {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                .AddField("Permissions", role.Permissions.ToPermissionString())
                .AddField("Data", $"Mentionable: {(role.IsMentionable ? "yes" : "no")}.\nHoisted: {(role.IsHoisted ? "yes" : "no")}.\nManaged: {(role.IsManaged ? "yes" : "no")}.")
                .WithColor(role.Color);

            if (!string.IsNullOrEmpty(role.IconUrl))
                embed.WithThumbnail(role.IconUrl);

            await context.ElevatedRespondAsync(embed: embed);
        }

        [Command("channel"), Aliases("c"), Description("Returns information about a specific channel."), CheckDisable]
        public async Task ChannelInfoAsync(CommandContext context, [Description("Channel to get information about")]DiscordChannel channel)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle($"#{channel.Name} ID: ({channel.Id})")
                .WithDescription($"Topic: {channel.Topic}\nCreated at: {channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}" +
                $"{(channel.ParentId != null ? $"\nChild of `{channel.Parent.Name.ToUpper()}` ID: ({channel.Parent.Id})" : "")}");

            if (channel.IsCategory)
            {
                var channelstring = new StringBuilder();
                #region channel list string builder
                foreach (var childchannel in channel.Children)
                {
                    switch (childchannel.Type)
                    {
                        case ChannelType.Text:
                            channelstring.Append($"[`#{childchannel.Name} (💬)`]");
                            break;
                        case ChannelType.Voice:
                            channelstring.Append($"`[{childchannel.Name} (🔈)]`");
                            break;
                        case ChannelType.Category:
                            channelstring.Append($"`[{childchannel.Name.ToUpper()} (📁)]`");
                            break;
                        default:
                            channelstring.Append($"`[{childchannel.Name} (❓)]`");
                            break;
                    }
                }
                #endregion
                embed.AddField("Children of category", channelstring.ToString());
            }
            if (channel.Type == ChannelType.Voice)
            {
                embed.AddField("Voice", $"Bit rate: {channel.Bitrate}\nUser limit: {(channel.UserLimit == 0 ? "Unlimited" : $"{channel.UserLimit}")}");
            }
            embed.AddField("Misc", $"NSFW: {(channel.IsNSFW ? "yes" : "no")}\n" +
                $"{(channel.Type == ChannelType.Text ? $"Last message ID: {channel.LastMessageId}" : "")}");

            await context.ElevatedRespondAsync(embed: embed);
        }
    }
}
