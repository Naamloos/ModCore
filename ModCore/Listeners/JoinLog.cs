using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
	public static class JoinLog
	{
		const string WelcomeRegex = "{{.*?}}";

		[AsyncListener(EventTypes.GuildMemberAdded)]
		public static async Task WelcomeNewMember(ModCoreShard bot, GuildMemberAddEventArgs e)
		{
			GuildSettings cfg = null;
			using (var db = bot.Database.CreateContext())
				cfg = e.Guild.GetGuildSettings(db);

			if (cfg != null && cfg.Welcome.Enabled)
			{
				if (cfg.Welcome.ChannelId != 0)
				{
					var c = (DiscordChannel)null;
					try
					{
						c = e.Guild.GetChannel((ulong)cfg.Welcome.ChannelId);
					}
					catch (Exception)
					{
						return;
					}

					string msg = cfg.Welcome.Message;
					string attachment = "";
					string embedtitle = "";
					bool isembed = false;

					Regex rgx = new Regex(WelcomeRegex);
					var match = rgx.Matches(msg);
					foreach (Match m in match)
					{
						var str = m.Value.Remove(m.Value.Length - 2).Substring(2);
						string replace = "";
						bool notinswitch = true;

						switch (str)
						{
							case "username":
								replace = e.Member.Username;
								notinswitch = false;
								break;

							case "discriminator":
								replace = e.Member.Discriminator;
								notinswitch = false;
								break;

							case "mention":
								replace = e.Member.Mention;
								notinswitch = false;
								break;

							case "userid":
								replace = e.Member.Id.ToString();
								notinswitch = false;
								break;

							case "guildname":
								replace = e.Guild.Name;
								notinswitch = false;
								break;

							case "channelname":
								replace = c.Name;
								notinswitch = false;
								break;

							case "membercount":
								replace = e.Guild.MemberCount.ToString();
								notinswitch = false;
								break;

							case "prefix":
								replace = cfg.Prefix ?? "?>";
								notinswitch = false;
								break;

							case "owner-username":
								replace = e.Guild.Owner.Username;
								notinswitch = false;
								break;

							case "owner-discriminator":
								replace = e.Guild.Owner.Discriminator;
								notinswitch = false;
								break;

							case "guild-icon-url":
								replace = e.Guild.IconUrl;
								notinswitch = false;
								break;

							case "channel-count":
								replace = e.Guild.Channels.Count.ToString();
								notinswitch = false;
								break;

							case "role-count":
								replace = e.Guild.Roles.Count.ToString();
								notinswitch = false;
								break;
						}

						msg = msg.Replace(m.Value, replace);

						if (notinswitch)
						{
							if (str.StartsWith("image:"))
								attachment = str.Substring(6);
							else if (str.StartsWith("embed-title:"))
								embedtitle = str.Substring(12);
							else if (str == "isembed")
								isembed = true;
						}
					}

					msg = rgx.Replace(msg, "");

					if (!isembed)
					{
						await c.SendMessageAsync($"{msg}\n\n{attachment}");
					}
					else
					{
						var eb = new DiscordEmbedBuilder()
							.WithDescription(msg);
						if (embedtitle != "")
							eb.WithTitle(embedtitle);
						if (attachment != "")
							eb.WithImageUrl(attachment);

						await c.SendMessageAsync(embed: eb);
					}
				}
			}
		}

		[AsyncListener(EventTypes.GuildMemberAdded)]
		public static async Task LogNewMember(ModCoreShard bot, GuildMemberAddEventArgs e)
		{
			GuildSettings cfg = null;
			using (var db = bot.Database.CreateContext())
				cfg = e.Guild.GetGuildSettings(db);
			if (cfg != null && cfg.JoinLog.Enable)
			{
				var m = e.Member;
				var c = (DiscordChannel)null;
				try
				{
					c = e.Guild.GetChannel((ulong)cfg.JoinLog.ChannelId);
				}
				catch (Exception)
				{
					return;
				}
				var embed = new DiscordEmbedBuilder()
					.WithTitle("New member joined")
					.WithDescription($"ID: ({m.Id})")
					.WithAuthor($"{m.Username}#{m.Discriminator}", icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl)
					.AddField("Join Date", $"{m.JoinedAt.DateTime.ToString()}")
					.AddField("Register Date", $"{m.CreationTimestamp.DateTime.ToString()}")
					.WithColor(DiscordColor.Green);
				await c.ElevatedMessageAsync(embed: embed);
			}
		}

		[AsyncListener(EventTypes.GuildMemberRemoved)]
		public static async Task LogLeaveMember(ModCoreShard bot, GuildMemberRemoveEventArgs e)
		{
			GuildSettings cfg = null;
			using (var db = bot.Database.CreateContext())
				cfg = e.Guild.GetGuildSettings(db);
			if (cfg.JoinLog.Enable)
			{
				var m = e.Member;
				var c = (DiscordChannel)null;
				try
				{
					c = e.Guild.GetChannel((ulong)cfg.JoinLog.ChannelId);
				}
				catch (Exception)
				{
					return;
				}
				var embed = new DiscordEmbedBuilder()
					.WithTitle("Member left")
					.WithDescription($"ID: ({m.Id})")
					.WithAuthor($"{m.Username}#{m.Discriminator}", icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl);
				if (m.JoinedAt.DateTime == DateTime.MinValue)
					embed.AddField("Join Date", $"{m.JoinedAt.DateTime.ToString()}");
				embed.AddField("Leave Date", $"{DateTime.Now.ToString()}")
				.AddField("Register Date", $"{m.CreationTimestamp.DateTime.ToString()}")
				.WithColor(DiscordColor.Red);
				await c.ElevatedMessageAsync(embed: embed);
			}
		}
	}
}
