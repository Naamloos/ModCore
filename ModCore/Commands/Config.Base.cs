using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DNTCaptcha.Core.Providers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Commands
{
    public partial class Config
    {
	    [GroupCommand, Description("Starts the ModCore configuration wizard. You probably want to do this first!")]
		public async Task ExecuteGroupAsync(CommandContext ctx)
		{
			await ctx.IfGuildSettingsAsync(async () =>
			{
				var t0 = await ctx.SafeRespondAsync(
					"Welcome to ModCore! Looks like you haven't configured your guild yet." +
					"Would you like to go through a quick setup? (Y/N)");

				var message = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
					TimeSpan.FromSeconds(40));
				if (!message.Message.Content.EqualsIgnoreCase("y") &&
					!message.Message.Content.EqualsIgnoreCase("yes") &&
					!message.Message.Content.EqualsIgnoreCase("ya") &&
					!message.Message.Content.EqualsIgnoreCase("ja") &&
					!message.Message.Content.EqualsIgnoreCase("da"))
				{
					await ctx.SafeRespondAsync(
						"OK, I won't bother you anymore. Just execute this command again if you need help configuring.");
					await t0.DeleteAsync("modcore cleanup after itself: welcome message");
					await message.Message.DeleteAsync(
						"modcore cleanup after itself: user response to welcome message");
					return;
				}

				DiscordChannel channel;
				try
				{
					channel =
						ctx.Guild.Channels.FirstOrDefault(e => e.Name == "modcore-setup") ??
						await ctx.Guild.CreateChannelAsync("modcore-setup", ChannelType.Text, null, null, null,
							null, null, "modcore setup channel creation");
				}
				catch
				{
					await ctx.SafeRespondAsync("Unfortunately, I wasn't able to create the modcore setup channel.\n" +
										   "Could you kindly create a channel called `modcore-setup` and re-run the command?\n" +
										   "I'll set up the rest for you. This will help keep the setup process away from prying eyes.");
					return;
				}
				await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None,
					Permissions.AccessChannels, "modcore overwrites for setup channel");
				await channel.AddOverwriteAsync(ctx.Member, Permissions.AccessChannels, Permissions.None,
					"modcore overwrites for setup channel");

				await channel.ElevatedMessageAsync(
					"OK, now, can you create a webhook for ModCore and give me its URL?\n" +
					"If you don't know what that is, simply say no and I'll make one for you.");

				var message2 = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
					TimeSpan.FromSeconds(40));
				var mContent = message2.Message.Content;
				if (!mContent.Contains("discordapp.com/api/webhooks/"))
				{
					await channel.ElevatedMessageAsync("Alright, I'll make a webhook for you then. Sit tight...");
					DiscordChannel logChannel;
					try
					{
						logChannel =
							ctx.Guild.Channels.FirstOrDefault(e => e.Name == "modlog") ??
							await ctx.Guild.CreateChannelAsync("modlog", ChannelType.Text, null, null, null,
								null, null, "ModCore Logging channel.");
					}
					catch
					{
						await ctx.SafeRespondAsync("Unfortunately, I wasn't able to create the modcore logging channel.\n" +
											   "Could you kindly create a channel called `modlog` and re-run the command?\n" +
											   "I'll set up the rest for you. This is to setup a channel for the actionlog to post into.");
						return;
					}

					var webhook = await logChannel.CreateWebhookAsync("ModCore Logging", null, "Created webhook to post log messages");

					var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
					cfg.ActionLog.WebhookId = webhook.Id;
					cfg.ActionLog.WebhookToken = webhook.Token;
					await ctx.SetGuildSettingsAsync(cfg);
				}
				else
				{
					var tokens = mContent
						.Substring(mContent.IndexOfInvariant("/api/webhooks/") + "/api/webhooks/".Length)
						.Split('/');

					var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
					cfg.ActionLog.WebhookId = ulong.Parse(tokens[0]);
					cfg.ActionLog.WebhookToken = tokens[1];
					await ctx.SetGuildSettingsAsync(cfg);
				}
				await ctx.SafeRespondAsync(
						"Webhook configured. Looks like you're all set! ModCore has been set up.");

			},
				async gcfg =>
				{
					var embed = new DiscordEmbedBuilder
					{
						Title = ctx.Guild.Name,
						ThumbnailUrl = ctx.Guild.IconUrl,
						Description = "ModCore configuration for this guild:"
					};

					embed.AddField("Command Prefix", gcfg.Prefix != null ? $"\"{gcfg.Prefix}\"" : "Not configured",
						true);

					var muted = gcfg.MuteRoleId != 0 ? ctx.Guild.GetRole(gcfg.MuteRoleId) : null;
					embed.AddField("Muted Role", muted != null ? muted.Mention : "Not configured or missing", true);

					var actionlog = gcfg.ActionLog;
					embed.AddField("Action Log",
						actionlog.Enable
							? "Enabled" + (actionlog.WebhookId == 0 ? ", but not configured!" : "")
							: "Disabled");

					var autorole = gcfg.AutoRole;
					embed.AddField("Auto Role",
						autorole.Enable ? $"Enabled with Role ID {autorole.RoleId}." : "Disabled");

					var commanderror = gcfg.CommandError;
					embed.AddField("Command Error logging",
						$"Chat: {commanderror.Chat}, ActionLog: {commanderror.ActionLog}");

					var linkfilterCfg = gcfg.Linkfilter;
					embed.AddField("Linkfilter", linkfilterCfg.Enable ? "Enabled" : "Disabled", true);
					if (linkfilterCfg.Enable)
					{
						embed.AddField("Linkfilter Modules",
							$"{(linkfilterCfg.BlockInviteLinks ? "✔" : "✖")}Anti-Invites, " +
							$"{(linkfilterCfg.BlockBooters ? "✔" : "✖")}Anti-DDoS, " +
							$"{(linkfilterCfg.BlockIpLoggers ? "✔" : "✖")}Anti-IP Loggers, " +
							$"{(linkfilterCfg.BlockShockSites ? "✔" : "✖")}Anti-Shock Sites, " +
							$"{(linkfilterCfg.BlockUrlShorteners ? "✔" : "✖")}Anti-URL Shorteners", true);

						if (linkfilterCfg.ExemptRoleIds.Any())
						{
							var roles = linkfilterCfg.ExemptRoleIds
								.Select(ctx.Guild.GetRole)
								.Where(xr => xr != null)
								.Select(xr => xr.Mention);

							embed.AddField("Linkfilter-Exempt Roles", string.Join(", ", roles), true);
						}

						if (linkfilterCfg.ExemptUserIds.Any())
						{
							var users = linkfilterCfg.ExemptUserIds
								.Select(xid => $"<@!{xid}>");

							embed.AddField("Linkfilter-Exempt Users", string.Join(", ", users), true);
						}

						if (linkfilterCfg.ExemptInviteGuildIds.Any())
						{
							var guilds = string.Join(", ", linkfilterCfg.ExemptInviteGuildIds);

							embed.AddField("Linkfilter-Exempt Invite Targets", guilds, true);
						}
					}

					var roleCfg = gcfg.RoleState;
					embed.AddField("Role State", roleCfg.Enable ? "Enabled" : "Disabled", true);
					if (roleCfg.Enable)
					{
						if (roleCfg.IgnoredRoleIds.Any())
						{
							var roles = roleCfg.IgnoredRoleIds
								.Select(ctx.Guild.GetRole)
								.Where(xr => xr != null)
								.Select(xr => xr.Mention);

							embed.AddField("Role State-Ignored Roles", string.Join(", ", roles), true);
						}

						if (roleCfg.IgnoredChannelIds.Any())
						{
							var channels = roleCfg.IgnoredChannelIds
								.Select(ctx.Guild.GetChannel)
								.Where(xc => xc != null)
								.Select(xc => xc.Mention);

							embed.AddField("Role State-Ignored Channels", string.Join(", ", channels), true);
						}
					}

					var inviscopCfg = gcfg.InvisiCop;
					embed.AddField("InvisiCop", inviscopCfg.Enable ? "Enabled" : "Disabled", true);
					if (inviscopCfg.Enable)
					{
						if (inviscopCfg.ExemptRoleIds.Any())
						{
							var roles = inviscopCfg.ExemptRoleIds
								.Select(ctx.Guild.GetRole)
								.Where(xr => xr != null)
								.Select(xr => xr.Mention);

							embed.AddField("InvisiCop-Exempt Roles", string.Join(", ", roles), true);
						}

						if (inviscopCfg.ExemptUserIds.Any())
						{
							var users = inviscopCfg.ExemptUserIds
								.Select(xid => $"<@!{xid}>");

							embed.AddField("InvisiCop-Exempt Users", string.Join(", ", users), true);
						}
					}
					embed.AddField("Starboard",
						gcfg.Starboard.Enable ? $"Enabled\nChannel: <#{gcfg.Starboard.ChannelId}>\nEmoji: {gcfg.Starboard.Emoji.EmojiName}" : "Disabled", true);

					var suggestions = gcfg.SpellingHelperEnabled;
					embed.AddField("Command Suggestions",
						suggestions ? "Enabled" : "Disabled", true);

					var joinlog = gcfg.JoinLog;
					embed.AddField("Join Logging",
						joinlog.Enable
							? "Enabled" + (joinlog.ChannelId == 0 ? ", but no channel configured!" : "")
							: "Disabled", true);

					if (gcfg.SelfRoles.Any())
					{
						var roles = gcfg.SelfRoles
							.Select(ctx.Guild.GetRole)
							.Where(xr => xr != null)
							.Select(xr => xr.Mention);

						embed.AddField("Available Selfroles", string.Join(", ", roles), true);
					}

					var globalwarn = gcfg.GlobalWarn;
					embed.AddField("GlobalWarn",
						globalwarn.Enable
							? "Enabled\nMode: " + globalwarn.WarnLevel
							: "Disabled", true);
					await ctx.ElevatedRespondAsync(embed: embed.Build());
				});
		}

	    [Command("reset"), Description("Resets this guild's configuration to initial state. This cannot be reversed.")]
	    public async Task ResetAsync(CommandContext ctx)
	    {
		    using (var db = this.Database.CreateContext())
		    {
			    var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == ctx.Guild.Id);
			    if (cfg == null)
			    {
				    await ctx.SafeRespondAsync("This guild is not configured.");
				    return;
			    }

			    var captcha = GetUniqueKey(32);
			    
			    await ctx.RespondWithFileAsync(
				    $"{GetUniqueKey(64)}.png",
				    new MemoryStream(CaptchaProvider.DrawCaptcha(captcha, "#99AAB5", "#23272A", 24, "Courier New")),
				    "You are about to reset the configuration for this guild. **This change is irreversible.** " +
				    "Type the characters in the image to continue. You have 45 seconds.");
			    
			    var msg = await this.Interactivity
				    .WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content.ToUpperInvariant() == captcha, TimeSpan.FromSeconds(45));
			    if (msg == null)
			    {
				    await ctx.SafeRespondAsync("Operation aborted.");
				    return;
			    }

			    db.GuildConfig.Remove(cfg);
			    await db.SaveChangesAsync();
		    }

		    await ctx.SafeRespondAsync("Configuration reset.");
	    }
	    
	    private string GetUniqueKey(int maxSize)
	    {
		    var chars = "ABCDEFGHMPRSTUVWXYZ23456789".ToCharArray();
		    var data = new byte[1];
		    
		    RandomNumberProvider.GetNonZeroBytes(data);
			data = new byte[maxSize];
		    RandomNumberProvider.GetNonZeroBytes(data);
		
		    var result = new StringBuilder(maxSize);
		    foreach (var b in data)
		    {
			    result.Append(chars[b % chars.Length]);
		    }
		    return result.ToString();
	    }
    }
}