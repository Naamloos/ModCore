using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;

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

        public SharedData Shared { get; }

        public Config(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive)
		{
			this.Database = db;
			this.Interactivity = interactive;
            this.Shared = shared;
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
    }
}