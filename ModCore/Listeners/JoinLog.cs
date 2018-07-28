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
		private static readonly Regex WelcomeRegex = new Regex("{{(.*?)}}", RegexOptions.Compiled);

		[AsyncListener(EventTypes.GuildMemberAdded)]
		public static async Task LogNewMember(ModCoreShard bot, GuildMemberAddEventArgs e)
		{
			GuildSettings cfg;
			using (var db = bot.Database.CreateContext())
				cfg = e.Guild.GetGuildSettings(db);

			DiscordChannel c;
			if (cfg.JoinLog.Enable)
			{
				var m = e.Member;
				c = e.Guild.GetChannel(cfg.JoinLog.ChannelId);
				if (c != null)
				{
					var embed = new DiscordEmbedBuilder()
						.WithTitle("New member joined")
						.WithDescription($"ID: ({m.Id})")
						.WithAuthor($"{m.Username}#{m.Discriminator}",
							icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl)
						.AddField("Join Date", $"{m.JoinedAt.DateTime}")
						.AddField("Register Date", $"{m.CreationTimestamp.DateTime}")
						.WithColor(DiscordColor.Green);
					await c.ElevatedMessageAsync(embed);
				}
			}

			if (!cfg.Welcome.Enable)
				return;

			if (cfg.Welcome.ChannelId == 0)
				return;

			c = e.Guild.GetChannel(cfg.Welcome.ChannelId);
			
			if (c == null)
				return;

			var msg = cfg.Welcome.Message;
			string attachment = null;
			string embedtitle = null;
			var isEmbed = false;

			msg = WelcomeRegex.Replace(msg, m =>
			{
				var str = m.Groups[1].Value;

				switch (str)
				{
					case "username":
						return e.Member.Username;

					case "discriminator":
						return e.Member.Discriminator;

					case "mention":
						return e.Member.Mention;

					case "userid":
						return e.Member.Id.ToString();

					case "guildname":
						return e.Guild.Name;

					case "channelname":
						return c.Name;

					case "membercount":
						return e.Guild.MemberCount.ToString();

					case "prefix":
						return cfg.Prefix ?? "?>";

					case "owner-username":
						return e.Guild.Owner.Username;

					case "owner-discriminator":
						return e.Guild.Owner.Discriminator;

					case "guild-icon-url":
						return e.Guild.IconUrl;

					case "channel-count":
						return e.Guild.Channels.Count.ToString();

					case "role-count":
						return e.Guild.Roles.Count.ToString();

					case "isembed":
						isEmbed = true;
						return "";

					default:
						if (str.StartsWith("image:"))
							attachment = str.Substring("image:".Length);
						else if (str.StartsWith("embed-title:"))
							embedtitle = str.Substring("embed-title:".Length);
						return "";
				}
			});

			if (!isEmbed)
			{
				await c.SafeMessageAsync(privileged: false, s: $"{msg}\n\n{attachment}");
			}
			else
			{
				var embed = new DiscordEmbedBuilder()
					.WithDescription(msg);
				
				if (!string.IsNullOrWhiteSpace(embedtitle))
					embed.WithTitle(embedtitle);
				if (!string.IsNullOrWhiteSpace(attachment))
					embed.WithImageUrl(attachment);

				await c.ElevatedMessageAsync(embed);
			}
		}

		[AsyncListener(EventTypes.GuildMemberRemoved)]
		public static async Task LogLeaveMember(ModCoreShard bot, GuildMemberRemoveEventArgs e)
		{
			GuildSettings cfg;
			using (var db = bot.Database.CreateContext())
				cfg = e.Guild.GetGuildSettings(db);
			
			if (!cfg.JoinLog.Enable)
				return;

			var m = e.Member;
			var c = e.Guild.GetChannel(cfg.JoinLog.ChannelId);

			if (c == null)
				return;

			var embed = new DiscordEmbedBuilder()
				.WithTitle("Member left")
				.WithDescription($"ID: ({m.Id})")
				.WithAuthor($"{m.Username}#{m.Discriminator}",
					icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl);

			if (m.JoinedAt.DateTime == DateTime.MinValue)
				embed.AddField("Join Date", $"{m.JoinedAt.DateTime}");

			embed
				.AddField("Leave Date", $"{DateTime.Now}")
				.AddField("Register Date", $"{m.CreationTimestamp.DateTime}")
				.WithColor(DiscordColor.Red);

			await c.ElevatedMessageAsync(embed);
		}
	}
}
