using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    [SlashCommandGroup("info", "List information about specific entities.")]
    public class Info : ApplicationCommandModule
    {
        [SlashCommand("member", "List information about a member.")]
        public async Task MemberAsync(InteractionContext ctx, [Option("user", "User to show information about.")]DiscordUser user)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"{member.Username}#{member.Discriminator} ({member.Id})");

            if (member.IsBot)
                embed.Title += " (BOT) ";

            if (member.IsOwner)
                embed.Title += " (OWNER) ";

            embed.Description =
                $"Registered: <t:{member.CreationTimestamp.ToUnixTimeSeconds()}:F>\n" +
                $"Joined: <t:{member.JoinedAt.ToUnixTimeSeconds()}:F>";

            var roles = new StringBuilder();

            foreach (var r in member.Roles)
                roles.Append($"`{r.Name.Replace("`", "'")}`");

            if (roles.Length == 0)
                roles.Append("*None*");

            var permissionsEnum = member.Permissions;

            var permissions = permissionsEnum.ToPermissionString();

            embed.AddField("Roles", roles.ToString());
            embed.AddField("Permissions", permissions);
            embed.WithThumbnail(member.GetGuildAvatarUrl(ImageFormat.Auto));

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("permissions", "List permissions for a specific user")]
        public async Task PermsAsync(InteractionContext ctx, [Option("user", "User to show information about.")] DiscordUser user)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"All permissions for {member.Username}#{member.Discriminator} ({member.Id})");

            embed.WithDescription(member.Permissions.ToPermissionString());
            embed.WithThumbnail(member.GetGuildAvatarUrl(ImageFormat.Auto));

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("channel-permissions", "List channel permissions for a specific user")]
        public async Task ChannelPermsAsync(InteractionContext ctx, [Option("user", "User to show information about.")] DiscordUser user)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"Permissions in current channel for {member.Username}#{member.Discriminator} ({member.Id})");

            var permissionsEnum = member.PermissionsIn(ctx.Channel);

            embed.WithDescription(permissionsEnum.ToPermissionString());
            embed.WithThumbnail(member.GetGuildAvatarUrl(ImageFormat.Auto));

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("role", "List information about a role.")]
        public async Task RoleAsync(InteractionContext ctx, [Option("role", "Role to show information about.")]DiscordRole role)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithTitle($"{role.Name} ID: ({role.Id})")
                .WithDescription($"Created at {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                .AddField("Permissions", role.Permissions.ToPermissionString())
                .AddField("Data", $"Mentionable: {(role.IsMentionable ? "yes" : "no")}.\nHoisted: {(role.IsHoisted ? "yes" : "no")}.\nManaged: {(role.IsManaged ? "yes" : "no")}.")
                .WithColor(role.Color);

            if (!string.IsNullOrEmpty(role.IconUrl))
                embed.WithThumbnail(role.IconUrl);

            if (!string.IsNullOrEmpty(role.IconUrl))
                embed.WithThumbnail(role.IconUrl);

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("server", "List information about the current server.")]
        public async Task ServerAsync(InteractionContext ctx)
        {
            var guild = ctx.Guild;
            var member = await guild.GetMemberAsync(ctx.User.Id);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"{guild.Name} ID: ({guild.Id})")
                .WithDescription($"Created: <t:{guild.CreationTimestamp.ToUnixTimeSeconds()}:F>\n" +
                $"Member count: {guild.MemberCount}\n" +
                $"Joined: <t:{guild.JoinedAt.ToUnixTimeSeconds()}:F>\n" +
                $"You joines: <t:{member.JoinedAt.ToUnixTimeSeconds()}:F>");

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

            await ctx.CreateResponseAsync(embed, true);
        }

        // TODO add new channel types
        [SlashCommand("channel", "List information about a channel")]
        public async Task ChannelAsync(InteractionContext ctx, [Option("channel", "Channel to list information about.")]DiscordChannel channel)
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
                $"{(channel.Type == ChannelType.Text ? $"Last message ID: {(await channel.GetMessagesAsync(1))[0].Id}" : "")}");

            await ctx.CreateResponseAsync(embed, true);
        }
    }
}
