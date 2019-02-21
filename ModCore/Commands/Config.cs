using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Commands.Base;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using ModCore.Logic.Utils.Captcha;

namespace ModCore.Commands
{
	[Group("config")]
	[Aliases("cfg")]
	[Description("Guild configuration options. Invoking without a subcommand will list current guild's settings.")]
	[RequireUserPermissions(Permissions.ManageGuild)]
	public partial class Config : BaseCommandModule
	{
		public static DiscordEmoji CheckMark { get; } = DiscordEmoji.FromUnicode("✅");

		public DatabaseContextBuilder Database { get; }
		public InteractivityExtension Interactivity { get; }

		private RandomNumberProvider RandomNumberProvider { get; } = new RandomNumberProvider();
		private CaptchaImageProvider CaptchaProvider { get; }
		
		public Config(DatabaseContextBuilder db, InteractivityExtension interactive)
		{
			this.Database = db;
			this.Interactivity = interactive;
			this.CaptchaProvider = new CaptchaImageProvider(RandomNumberProvider);
		}

		[Command("prefix"), Aliases("pfix"),
		 Description("Sets the command prefix for this guild. Prefixes longer than 10 characters will be truncated.")]
		public async Task PrefixAsync(CommandContext ctx,
			[Description("New command prefix for this guild")] string prefix = null)
		{
			if (string.IsNullOrWhiteSpace(prefix))
				prefix = null;

			prefix = prefix?.TrimStart();
			if (prefix?.Length > 20)
				prefix = prefix.Substring(0, 21);

			await ctx.WithGuildSettings(cfg => cfg.Prefix = prefix);

			await ctx.SafeRespondAsync(prefix == null
				? "Prefix restored to default."
				: $"Prefix changed to: \"{prefix}\".");
		}

		[Command("muterole"), Aliases("mr"),
		 Description("Sets the role used to mute users. Invoking with no arguments will reset this setting.")]
		public async Task MuteRole(CommandContext ctx,
			[Description("New mute role for this guild")] DiscordRole role = null)
		{
			await ctx.WithGuildSettings(cfg => cfg.MuteRoleId = role == null ? 0 : role.Id);

			await ctx.SafeRespondAsync(role == null ? "Muting disabled." : $"Mute role set to {role.Mention}.");
		}

		[Group("suggestions"), Aliases("suggestion", "sugg", "sug", "s"), Description("Suggestions configuration commands.")]
		public class Suggestions : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.SpellingHelperEnabled;
		}

		[Group("linkfilter"), Aliases("inviteblocker", "invite", "ib", "filter", "lf"),
		 Description("Linkfilter configuration commands.")]
		public class Linkfilter : SimpleConfigModule
		{
			// TODO add wizard for this...
			
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.Linkfilter.Enable;
			
			[Group("modules"), Aliases("mod", "m", "s"),
			 Description("Commands to toggle Linkfilter modules for this guild.\n" +
			             "You can enable/disable modules by using the module name and `enable`, `disable` or leave " +
			             "empty to toggle.")]
			public class Modules : BaseCommandModule
			{
				[Group("all"), Aliases("a", "0"), Description("Commands to manage all Linkfilter modules at once.")]
				public class AllModules : BaseCommandModule
				{
					[Command("off"), Aliases("disable", "disabled", "0", "false", "no", "n"),
					 Description("Disables all Linkfilter modules for this guild.")]
					public async Task DisableAllLinkfilterModulesAsync(CommandContext ctx)
					{
						await ctx.WithGuildSettings(cfg =>
						{
							cfg.Linkfilter.BlockBooters = false;
							cfg.Linkfilter.BlockInviteLinks = false;
							cfg.Linkfilter.BlockIpLoggers = false;
							cfg.Linkfilter.BlockShockSites = false;
							cfg.Linkfilter.BlockUrlShorteners = false;
						});
						await ctx.Message.CreateReactionAsync(CheckMark);
					}

					[Command("on"), Aliases("enable", "enabled", "1", "true", "yes", "y"),
					 Description("Enables all Linkfilter modules for this guild.")]
					public async Task EnableAllLinkfilterModulesAsync(CommandContext ctx)
					{
						await ctx.WithGuildSettings(cfg =>
						{
							cfg.Linkfilter.BlockBooters = true;
							cfg.Linkfilter.BlockInviteLinks = true;
							cfg.Linkfilter.BlockIpLoggers = true;
							cfg.Linkfilter.BlockShockSites = true;
							cfg.Linkfilter.BlockUrlShorteners = true;
						});
						await ctx.Message.CreateReactionAsync(CheckMark);
					}

					[Command("default"), Aliases("def", "d", "2"),
					 Description("Sets all Linkfilter modules to default for this guild.")]
					public async Task RestoreDefaultAllLinkfilterModulesAsync(CommandContext ctx)
					{
						await ctx.WithGuildSettings(cfg =>
						{
							cfg.Linkfilter.BlockBooters = true;
							cfg.Linkfilter.BlockInviteLinks = true;
							cfg.Linkfilter.BlockIpLoggers = true;
							cfg.Linkfilter.BlockShockSites = true;
							cfg.Linkfilter.BlockUrlShorteners = false;
						});
						await ctx.Message.CreateReactionAsync(CheckMark);
					}
				}

				[Group("booters"), Aliases("booter", "boot", "ddos", "b", "1"),
				 Description("Toggle blocking booter/DDoS sites for this guild.")]
				public class Booters : LinkfilterConfigModule
				{
					protected override ref bool GetSetting(GuildLinkfilterSettings lf) => ref lf.BlockBooters;
				}
				
				[Group("invites"), Aliases("invitelinks", "invite", "inv", "i", "2"),
				 Description("Toggle blocking invite links for this guild.")]
				public class InviteLinks : LinkfilterConfigModule
				{
					protected override ref bool GetSetting(GuildLinkfilterSettings lf) => ref lf.BlockInviteLinks;
				}

				[Group("iploggers"), Aliases("iplogs", "ips", "ip", "3"),
				 Description("Toggle blocking IP logger sites for this guild.")]
				public class IpLoggers : LinkfilterConfigModule
				{
					protected override ref bool GetSetting(GuildLinkfilterSettings lf) => ref lf.BlockIpLoggers;
				}

				[Group("shocksites"), Aliases("shock", "shocks", "gore", "g", "4"),
				 Description("Toggle blocking shock/gore sites for this guild.")]
				public class ShockSites : LinkfilterConfigModule
				{
					protected override ref bool GetSetting(GuildLinkfilterSettings lf) => ref lf.BlockShockSites;
				}

				[Group("urlshorteners"), Aliases("urlshortener", "urlshort", "urls", "url", "u", "5"),
				 Description("Toggle blocking URL shortener links for this guild.")]
				public class UrlShorteners : LinkfilterConfigModule
				{
					protected override ref bool GetSetting(GuildLinkfilterSettings lf) => ref lf.BlockUrlShorteners;
				}
			}

			[Group("user"), Aliases("usr", "u"), Description("User exemption management commands.")]
			public class User : ExemptMemberModule
			{
				protected override ISet<ulong> GetExemptionList(GuildSettings cfg) => cfg.Linkfilter.ExemptUserIds;
			}

			[Group("role"), Aliases("r"), Description("Role exemption management commands.")]
			public class Role : ExemptRoleModule
			{
				protected override ISet<ulong> GetExemptionList(GuildSettings cfg) => cfg.Linkfilter.ExemptRoleIds;
			}

			[Group("guild"), Aliases("invite", "i"), Description("Invite target exemption management commands.")]
			public class Guild : BaseCommandModule
			{
				[Command("exempt"), Aliases("x"), Description("Exempts code from invite checks.")]
				public async Task ExemptAsync(CommandContext ctx,
					[RemainingText, Description("Invite code to exempt from invite checks")] string invite)
				{
					var inv = await ctx.Client.GetInviteByCodeAsync(invite);
					if (inv == null)
					{
						await ctx.SafeRespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
						return;
					}

					await ctx.WithGuildSettings(cfg => cfg.Linkfilter.ExemptInviteGuildIds.Add(inv.Guild.Id));
					await ctx.Message.CreateReactionAsync(CheckMark);
				}

				[Command("unexempt"), Aliases("ux"), Description("Unexempts code from invite checks.")]
				public async Task UnexemptAsync(CommandContext ctx,
					[RemainingText, Description("Invite code to unexempt from invite checks")] string invite)
				{
					var inv = await ctx.Client.GetInviteByCodeAsync(invite);
					if (inv == null)
					{
						await ctx.SafeRespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
						return;
					}

					await ctx.WithGuildSettings(cfg => cfg.Linkfilter.ExemptInviteGuildIds.Remove(inv.Guild.Id));
					await ctx.Message.CreateReactionAsync(CheckMark);
				}
			}
		}

		[Group("rolestate"), Aliases("rs"), Description("Role State configuration commands.")]
		public class RoleState : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.RoleState.Enable;
			
			[Group("role"), Aliases("r"), Description("Role exemption management commands.")]
			public class Role : ExemptRoleModule
			{
				protected override ISet<ulong> GetExemptionList(GuildSettings cfg) => cfg.RoleState.IgnoredRoleIds;
			}

            [Group("nickname"), Aliases("n", "nick"), Description("Whether to enable or disable nickname recovery")]
            public class Nick : SimpleConfigModule
            {
                protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.RoleState.Nickname;
            }

			[Group("channel"), Aliases("c"), Description("Channel exemption management commands.")]
			public class Channel : ExemptChannelModule
			{
				private DatabaseContextBuilder Database { get; }

				public Channel(DatabaseContextBuilder db) => this.Database = db;

				protected override ISet<ulong> GetExemptionList(GuildSettings cfg) => cfg.RoleState.IgnoredChannelIds;

				protected override async Task AfterExemptAsync(CommandContext ctx, DiscordChannel chn)
				{
					using (var db = this.Database.CreateContext())
					{
						var chperms = db.RolestateOverrides.Where(xs =>
							xs.ChannelId == (long)chn.Id && xs.GuildId == (long)chn.Guild.Id);
						if (chperms.Any())
						{
							db.RolestateOverrides.RemoveRange(chperms);
							await db.SaveChangesAsync();
						}
					}
				}

				protected override async Task AfterUnexemptAsync(CommandContext ctx, DiscordChannel chn)
				{
					var os = chn.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToArray();
					using (var db = this.Database.CreateContext())
					{
						if (os.Any())
						{
							await db.RolestateOverrides.AddRangeAsync(os.Select(xo => new DatabaseRolestateOverride
							{
								ChannelId = (long)chn.Id,
								GuildId = (long)chn.Guild.Id,
								MemberId = (long)xo.Id,
								PermsAllow = (long)xo.Allowed,
								PermsDeny = (long)xo.Denied
							}));
							await db.SaveChangesAsync();
						}
					}
				}
			}
		}

		[Group("invisicop"), Aliases("ic"), Description("InvisiCop configuration commands.")]
		public class InvisiCop : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.InvisiCop.Enable;
		}

		[Group("actionlog"), Aliases("al"), Description("ActionLog configuration commands.")]
		public class ActionLog : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.ActionLog.Enable;
			
			protected override Task AfterEnable(CommandContext ctx) => ctx.ElevatedRespondAsync(
				"ActionLog enabled.\n" +
				"If you haven't done this yet, Please execute the `config actionlog setwebhook` command.");

			[Command("setwebhook"), Aliases("webhook", "hook", "swh"),
			 Description("Sets the webhook ID and token for this guild's action log.")]
			public async Task SetWebhookAsync(CommandContext ctx, [Description("Webhook ID")]ulong id, [Description("Webhook token")]string token)
			{
				await ctx.WithGuildSettings(cfg =>
				{
					cfg.ActionLog.WebhookId = id;
					cfg.ActionLog.WebhookToken = token;
				});
				await ctx.ElevatedRespondAsync("ActionLog webhook configured.");
			}
		}

		[Group("autorole"), Aliases("ar"), Description("AutoRole configuration commands.")]
		public class AutoRole : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.AutoRole.Enable;

			protected override Task AfterEnable(CommandContext ctx) => ctx.ElevatedRespondAsync(
					"AutoRole enabled.\n" +
					"If you haven't done this yet, Please execute the `config autorole setrole` command.");

			[Command("setrole"), Aliases("sr"),
			 Description("Sets a role to grant to new members.")]
			public async Task SetRoleAsync(CommandContext ctx, [Description("Role to grant to new members")]DiscordRole role)
			{
				await ctx.WithGuildSettings(cfg => cfg.AutoRole.RoleId = role.Id);
				await ctx.ElevatedRespondAsync("AutoRole role configured.");
			}
		}

		[Group("error"), Aliases("er"), Description("Error verbosity configuration commands.")]
		public class ErrorVerbosity : BaseCommandModule
		{
			[Command("chat"), Aliases("c"), Description("Sets command error reporting for this guild (in chat).")]
			public async Task ChatAsync(CommandContext ctx, [Description("New error verbosity")]string verbosity)
			{
				if (!await TryGetVerbosity(ctx, verbosity, out var vb)) return;
				
				await ctx.WithGuildSettings(cfg => cfg.CommandError.Chat = vb);
				await ctx.ElevatedRespondAsync($"Error reporting verbosity in chat set to `{verbosity}`.");
			}

			[Command("log"), Aliases("a"), Description("Sets command error reporting for this guild (in ActionLog).")]
			public async Task ActionLogAsync(CommandContext ctx, [Description("New error verbosity")]string verbosity)
			{
				if (!await TryGetVerbosity(ctx, verbosity, out var vb)) return;
				
				await ctx.WithGuildSettings(cfg => cfg.CommandError.ActionLog = vb);
				await ctx.ElevatedRespondAsync($"Error reporting verbosity in ActionLog set to `{verbosity}`.");
			}

			private static Task<bool> TryGetVerbosity(CommandContext ctx, string verbosity, out CommandErrorVerbosity vb)
			{
				switch (verbosity)
				{
					case "none":
						vb = CommandErrorVerbosity.None;
						return Task.FromResult(true);
					case "name":
						vb = CommandErrorVerbosity.Name;
						return Task.FromResult(true);
					case "namedesc":
						vb = CommandErrorVerbosity.NameDesc;
						return Task.FromResult(true);
					case "exception" when ctx.Client.CurrentApplication.Owner.Id == ctx.Member.Id:
						vb = CommandErrorVerbosity.Exception;
						return Task.FromResult(true);
					default:
						vb = CommandErrorVerbosity.None;
						
						if (ctx.Client.CurrentApplication.Owner.Id == ctx.Member.Id)
						{
							return ctx.ElevatedRespondAsync(
									"Unsupported verbosity level.\n" +
									"Supported levels: `none`, `name`, `namedesc` or `exception`")
								.ContinueWith(task => false);
						}

						return ctx.ElevatedRespondAsync(
								"Unsupported verbosity level.\n" +
								"Supported levels: `none`, `name` or `namedesc`")
							.ContinueWith(task => false);
				}
			}
		}

		[Group("joinlog"), Aliases("j"), Description("JoinLog configuration commands.")]
		public class JoinLog : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.JoinLog.Enable;

			protected override Task AfterEnable(CommandContext ctx) => ctx.ElevatedRespondAsync(
					"Joinlog enabled.\n" +
					"If you haven't done this yet, Please execute `config joinlog setchannel` command");

			[Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for this guild's JoinLog.")]
			public async Task SetChannelAsync(CommandContext ctx, [Description("Channel to send the JoinLogs to")]DiscordChannel channel)
			{
				await ctx.WithGuildSettings(cfg => cfg.JoinLog.ChannelId = channel.Id);
				await ctx.SafeRespondAsync("JoinLog channel configured.");
			}
		}

		[Group("selfrole"), Aliases("sr"), Description("SelfRole configuration commands.")]
		public class SelfRole : BaseCommandModule
		{
			[Command("add"), Aliases("a"), Description("Adds roles to SelfRole list")]
			public async Task AddSelfRoleAsync(CommandContext ctx, [Description("Role to allow for self-granting")]DiscordRole role)
			{
				await ctx.WithGuildSettings(cfg => 
					ctx.SafeRespondAsync(cfg.SelfRoles.Add(role.Id)
						? $"Added role `{role.Name}` with ID `{role.Id}` to SelfRoles." 
						: "This role has already been added!"));
			}

			[Command("remove"), Aliases("r"), Description("Removes roles from SelfRole list")]
			public async Task RemoveSelfRoleAsync(CommandContext ctx, [Description("Role to disallow from self-granting")]DiscordRole role)
			{
				await ctx.WithGuildSettings(cfg =>
					ctx.SafeRespondAsync(cfg.SelfRoles.Remove(role.Id)
						? $"Removed role `{role.Name}` with ID `{role.Id}` from SelfRoles."
						: "This role isn't in SelfRoles!"));
			}
		}

		[Group("starboard"), Aliases("star"), Description("Starboard configuration commands.")]
		public class Starboard : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.Starboard.Enable;

			protected override Task AfterEnable(CommandContext ctx) => ctx.ElevatedRespondAsync(
				"Starboard enabled.\n" +
				"If you haven't done this yet, Please execute `config starboard setchannel` command");
			
			[Group("allownsfw"), Aliases("nsfw"), Description("Sets whether or not to allow NSFW stars in this guild.")]
			public class AllowNsfw : SimpleConfigModule
			{
				protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.Starboard.AllowNSFW;
			}

			[Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for this guild's Starboard.")]
			public async Task SetChannelAsync(CommandContext ctx, [Description("Channel to log stars to")]DiscordChannel channel)
			{
				await ctx.WithGuildSettings(cfg => cfg.Starboard.ChannelId = channel.Id);
				await ctx.ElevatedRespondAsync("Starboard channel configured.");
			}

			[Command("setemoji"), Aliases("se"), Description("Sets the Starboard emoji for this guild.")]
			public async Task SetEmojiAsync(CommandContext ctx, [Description("Starboard emoji")]DiscordEmoji emoji)
			{
				await ctx.WithGuildSettings(cfg => 
					cfg.Starboard.Emoji = new GuildEmoji { EmojiId = emoji.Id, EmojiName = emoji.Name });
				await ctx.ElevatedRespondAsync($"Starboard emoji set to {emoji}.");
			}
		}

		[Group("reactionrole"), Aliases("rr"), Description("ReactionRole configuration commands.")]
		public class ReactionRole : BaseCommandModule
		{
			[Command("add"), Aliases("a"), Description("Add a reaction role to this guild.")]
			public async Task AddAsync(CommandContext ctx, ulong msgId, DiscordChannel chan, DiscordRole role, DiscordEmoji emoji)
			{
				await ctx.WithGuildSettings(async cfg =>
				{
					// Checks whether there's no existing reactionrole that has the same:
					// Channel, Message AND Reaction or Role.
					if (!cfg.ReactionRoles.Any(
						x => x.ChannelId == chan.Id
						     && x.MessageId == msgId
						     && (x.RoleId == role.Id 
						         || x.Reaction.EmojiId == emoji.Id 
						         && x.Reaction.EmojiName == emoji.Name)
						     ))
					{
						cfg.ReactionRoles.Add(new GuildReactionRole
						{
							ChannelId = chan.Id,
							MessageId = msgId,
							RoleId = role.Id,
							Reaction = new GuildEmoji
							{
								EmojiId = emoji.Id,
								EmojiName = emoji.Name
							}
						});
						var msg = await chan.GetMessageAsync(msgId);
						await msg.CreateReactionAsync(emoji);
						await ctx.ElevatedRespondAsync("New reactionrole added!");
					}
					else
					{
						await ctx.ElevatedRespondAsync(
							"You can't do that! That message already has a reactionrole with that role or reaction!");
					}
				});
			}

			[Command("remove"), Aliases("r"), Description("Removes a reaction role from this guild.")]
			public async Task RemoveAsync(CommandContext ctx, DiscordChannel chnl, ulong msgId, DiscordRole role)
			{
				await ctx.WithGuildSettings(async cfg =>
				{
					if (cfg.ReactionRoles.RemoveAll(x =>
						    x.ChannelId == chnl.Id && x.MessageId == msgId && x.RoleId == role.Id) > 0)
						await ctx.ElevatedRespondAsync("Removed reactionrole!");
					else
						await ctx.ElevatedRespondAsync("No reaction was linked to that role!");
				});
			}
		}

		[Group("globalwarn"), Aliases("gw"), Description("GlobalWarn configuration commands.")]
		public class GlobalWarn : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.GlobalWarn.Enable;

			[Command("changemode"), Aliases("cm"),
			 Description("Sets the GlobalWarn mode.")]
			public async Task SetRoleAsync(CommandContext ctx)
			{
				await ctx.SafeRespondAsync(@"__**Available GlobalWarn modes:**__
1. None
2. Owner (Warns the Server Owner if someone on the GlobalWarn list joins the server)
3. Joinlog (Sends the warning to the JoinLog channel)

Type an option.");
				var iv = ctx.Client.GetInteractivity();

				var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id &&
					(xm.Content.ToLower() == "none" || xm.Content.ToLower() == "owner" || xm.Content.ToLower() == "joinlog" || xm.Content == "1" || xm.Content == "2" || xm.Content == "3"),
				TimeSpan.FromSeconds(45));
				if (msg == null)
				{
					await ctx.SafeRespondAsync("Operation aborted.");
					return;
				}
				
				await ctx.WithGuildSettings(async cfg =>
				{
					switch (msg.Message.Content)
					{
						case "none":
						case "1":
							cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.None;
							break;
						case "owner":
						case "2":
							cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.Owner;
							break;
						case "joinlog":
						case "3":
							cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.JoinLog;
							break;
					}
					
					await ctx.ElevatedRespondAsync($"GlobalWarn mode configured to {cfg.GlobalWarn.WarnLevel}");
				});
			}
		}

		[Group("commandsettings"), Aliases("commands", "cmds", "cs", "c"), Description("Command settings commands.")]
		public class CommandSettings : BaseCommandModule
		{
			public DatabaseContextBuilder Database { get; }

			public CommandSettings(DatabaseContextBuilder db)
			{
				this.Database = db;
			}
			
			[Command("disable"), Aliases("d")]
			public async Task DisableAsync(CommandContext ctx, [RemainingText] string cmd)
			{
				var command = cmd.ToLowerInvariant();
				if (command.StartsWith("config"))
				{
					await ctx.ElevatedRespondAsync("You can't disable configuration commands!");
					return;
				}
				if (command.StartsWith("owner"))
				{
					await ctx.ElevatedRespondAsync("You can't disable owner commands!");
					return;
				}
				if (ctx.CommandsNext.RegisteredCommands.Any(x => CheckCommand(command, x.Value)))
				{
                    await ctx.WithGuildSettings(async cfg =>
                    {
                        using (var db = Database.CreateContext())
					    {
						    cfg.DisabledCommands.Add((await db.CommandIds.FindAsync(command)).Id);
					    }
                    });
					await ctx.SafeRespondAsync($"Disabled command `{command}` from use in this guild!");
				}
				else
				{
					await ctx.SafeRespondAsync($"No such command! `{command}`");
				}
			}

			public static bool CheckCommand(string command, Command cmd)
			{
				if (cmd.QualifiedName.StartsWith(command))
					return true;
				if (cmd is CommandGroup group)
					return group.Children.Any(e => CheckCommand(command, e));
				return false;
			}

			[Command("enable"), Aliases("e")]
			public async Task EnableAsync(CommandContext ctx, [RemainingText]string cmd)
			{
				var command = cmd.ToLowerInvariant();
				await ctx.WithGuildSettings(async cfg =>
				{
					short id = 0; 
					using (var db = Database.CreateContext())
					{
						id = (await db.CommandIds.FindAsync(command)).Id;
					}

					if (cfg.DisabledCommands.RemoveWhere(x => x == id) > 0)
						await ctx.SafeRespondAsync($"Enabled command `{command}` in this guild!");
					else
						await ctx.ElevatedRespondAsync("That command does not exist!");
				});
			}

			[Command("list"), Aliases("l")]
			public async Task ListAsync(CommandContext ctx)
			{
				var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
				if (cfg.DisabledCommands.Count == 0)
				{
					await ctx.ElevatedRespondAsync("No commands are currently disabled!");
					return;
				}
				// TODO List disabled commands
			}
			
			[Group("notify"), Aliases("warn", "not", "log", "n", "w", "l"), 
			 Description("Whether or not to send a message in chat when someone tries to execute a disabled command.")]
			public class Notify : SimpleConfigModule
			{
				protected override string CurrentModuleName => "notify on disabled command call";
				
				protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.NotifyDisabledCommand;
			}
		}

		[Group("welcome"), Aliases("w"), Description("Welcome message settings commands.")]
		public class Welcome : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.Welcome.Enable;

			[Command("set"), Aliases("setmessage"),
			 Description(@"Sets welcome message.
Welcome messages support a handful of tags that get parsed to their actual values:
{{username}}, {{discriminator}}, {{mention}}, {{userid}},
{{guildname}}, {{channelname}}, {{membercount}}, {{prefix}},
{{owner-username}}, {{owner-discriminator}}, {{guild-icon-url}}, {{channel-count}}, {{role-count}},
{{attach:url}}, {{embed-title:title}}, {{isembed}}")]
			public async Task SetMessageAsync(CommandContext ctx, string message)
			{
				await ctx.WithGuildSettings(cfg => cfg.Welcome.Message = message);
				await ctx.ElevatedRespondAsync("Set the welcome message.");
			}

			[Command("setchannel"), Aliases("setc", "sc"), Description("Set channel to send welcome messages to.")]
			public async Task SetChannel(CommandContext ctx, DiscordChannel channel)
			{
				await ctx.WithGuildSettings(cfg => cfg.Welcome.ChannelId = channel.Id);
				await ctx.ElevatedRespondAsync($"Set welcome channel to {channel.Mention}.");
			}
		}

		[Group("nameconfirm"), Aliases(
			 "nicknamechangingconfirmation", "namechangingconfirmation",
			 "nickchangingconfirmation", 
			 "nicknamechangeconfirmation", "namechangeconfirmation",
			 "nickchangeconfirmation",

			 "nicknamechangingconfirm", "namechangingconfirm", "nickchangingconfirm",
			 "nicknamechangeconfirm", "namechangeconfirm", "nickchangeconfirm",

			 "nicknameconfirmation", "nameconfirmation", "nickconfirmation",
			 "nicknameconfirm", "nickconfirm",

			 "nicknamechanging", "namechanging", "nickchanging",
			 "nicknamechange", "namechange", "nickchange",

			 "nnf", "nncf", "ncf", "nf", "nnc", "nc", "n"), Description("Nickname change requests system settings.")]
		public class NicknameChanging : SimpleConfigModule
		{
			protected override ref bool GetSetting(GuildSettings cfg) => ref cfg.RequireNicknameChangeConfirmation;

			protected override Task AfterEnable(CommandContext ctx) => ctx.ElevatedRespondAsync(
				"Nickname change confirmations enabled.\n" +
				"If you haven't done this yet, Please execute `config nicknamechange setchannel` command");

			[Command("setchannel"), Aliases("set-channel", "channel", "chan", "sc"),
			 Description("The channel where confirmations will go to. Anyone with access to this channel will be " +
			             "able to approve or deny nickname change requests, so choose wisely!")]
			public async Task SetChannel(CommandContext ctx, DiscordChannel channel)
			{
				await ctx.WithGuildSettings(cfg => cfg.NicknameChangeConfirmationChannel = channel.Id);
				await ctx.ElevatedRespondAsync($"Set nickname request confirmation channel to {channel.Mention}.");
			}
		}
	}
}