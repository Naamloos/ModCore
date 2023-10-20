﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
	public static class JoinLog
	{
		private static readonly Regex WelcomeRegex = new("{{(.*?)}}", RegexOptions.Compiled);

		[AsyncListener(EventType.GuildMemberAdded)]
		public static async Task LogNewMember(GuildMemberAddEventArgs eventargs, DatabaseContextBuilder database)
		{
			GuildSettings config;
			await using (var db = database.CreateContext())
				config = eventargs.Guild.GetGuildSettings(db);

            if(config == null)
            {
                return;
            }

			DiscordChannel channel;
			if (config.Logging.JoinLog_Enable)
			{
				var m = eventargs.Member;
				channel = eventargs.Guild.GetChannel(config.Logging.ChannelId);
				if (channel != null)
				{
					var newUser = DateTimeOffset.Now.Subtract(m.CreationTimestamp.DateTime).TotalDays < 30;
					var embed = new DiscordEmbedBuilder()
						.WithTitle("New member joined")
						.WithDescription($"ID: ({m.Id})" + (newUser? "\n\n⚠️ This is a very new user!" : ""))
						.WithAuthor($"{m.GetDisplayUsername()}",
							iconUrl: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl)
						.AddField("Join Date", m.JoinedAt == default? "Unknown" : m.JoinedAt.ToString())
						.AddField("Register Date", m.CreationTimestamp == default? "Unknown" : m.CreationTimestamp.ToString())
						.WithColor(newUser ? DiscordColor.Red : DiscordColor.Green)
                        .AddField("IDs", $"```ini\nUser = {eventargs.Member.Id}```"); ;
					await channel.ElevatedMessageAsync(embed);
				}
			}

			if (!config.Welcome.Enable)
				return;

			if (config.Welcome.ChannelId == 0)
				return;

			channel = eventargs.Guild.GetChannel(config.Welcome.ChannelId);
			
			if (channel == null)
				return;

			var message = config.Welcome.Message;
			if (string.IsNullOrEmpty(message))
				return;

			string attachment = null;
			string embedtitle = null;
			var isEmbed = config.Welcome.IsEmbed;

			message = WelcomeRegex.Replace(message, match =>
			{
				var welcome = match.Groups[1].Value;

				switch (welcome)
				{
					case "username":
						return eventargs.Member.Username;

					case "mention":
						return eventargs.Member.Mention;

					case "userid":
						return eventargs.Member.Id.ToString();

					case "guildname":
						return eventargs.Guild.Name;

					case "channelname":
						return channel.Name;

					case "membercount":
						return eventargs.Guild.MemberCount.ToString();

					case "owner-username":
						return eventargs.Guild.Owner.Username;

					case "guild-icon-url":
						return eventargs.Guild.IconUrl;

					case "channel-count":
						return eventargs.Guild.Channels.Count.ToString();

					case "role-count":
						return eventargs.Guild.Roles.Count.ToString();

					default:
						if (welcome.StartsWith("image:"))
							attachment = welcome.Substring("image:".Length);
						else if (welcome.StartsWith("embed-title:"))
							embedtitle = welcome.Substring("embed-title:".Length);
						return "";
				}
			});

			if (!isEmbed)
			{
				await channel.SafeMessageAsync(privileged: false, s: $"{message}\n\n{attachment}");
			}
			else
			{
				var embed = new DiscordEmbedBuilder()
					.WithDescription(message);
				
				if (!string.IsNullOrWhiteSpace(embedtitle))
					embed.WithTitle(embedtitle);
				if (!string.IsNullOrWhiteSpace(attachment))
					embed.WithImageUrl(attachment);

				await channel.ElevatedMessageAsync(embed);
			}
		}

		[AsyncListener(EventType.GuildMemberRemoved)]
		public static async Task LogLeaveMember(GuildMemberRemoveEventArgs eventargs, DatabaseContextBuilder database)
		{
			GuildSettings config;
			await using (var db = database.CreateContext())
				config = eventargs.Guild.GetGuildSettings(db);
			
			if (config == null || !config.Logging.JoinLog_Enable)
				return;

			var member = eventargs.Member;
			var channel = eventargs.Guild.GetChannel(config.Logging.ChannelId);

			if (channel == null)
				return;

			var embed = new DiscordEmbedBuilder()
				.WithTitle("Member left")
				.WithDescription($"ID: ({member.Id})")
				.WithAuthor($"{member.GetDisplayUsername()}",
					iconUrl: string.IsNullOrEmpty(member.AvatarHash) ? member.DefaultAvatarUrl : member.AvatarUrl)
                .AddField("IDs", $"```ini\nUser = {eventargs.Member.Id}\n```"); ;

			if (member.JoinedAt.DateTime == DateTime.MinValue)
				embed.AddField("Join Date", member.JoinedAt == default? "Unknown" : member.JoinedAt.ToString());
            embed
                .AddField("Leave Date", $"{DateTime.Now}")
				.AddField("Register Date", member.CreationTimestamp == default? "Unknown" : member.CreationTimestamp.ToString())
				.WithColor(DiscordColor.LightGray);

			await channel.ElevatedMessageAsync(embed);
		}
	}
}
