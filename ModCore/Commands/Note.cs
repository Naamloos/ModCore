using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ModCore.Commands
{
	// TODO: Reconsider required permissions
	[Group("note"), Aliases("n"), Description("Information commands"), CheckDisable, RequireUserPermissions(DSharpPlus.Permissions.BanMembers)]
	public class Note : BaseCommandModule
	{
		public SharedData Shared { get; }
		public InteractivityExtension Interactivity { get; }
		public DatabaseContextBuilder Database { get; }

		public Note(SharedData shared, InteractivityExtension interactive, DatabaseContextBuilder db)
		{
			this.Shared = shared;
			this.Interactivity = interactive;
			this.Database = db;
		}

		[GroupCommand]
		public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Member to get notes about.")]DiscordUser user)
		{
			await ctx.Message.DeleteAsync();
			var dbctx = Database.CreateContext();
			
			if (!dbctx.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id))
			{
				await ctx.Member.SendMessageAsync($"**No notes exist for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`!**");
			}
			else
			{
				var note = dbctx.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id);
				await ctx.Member.SendMessageAsync($"**Notes for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`:**\n```\n{note.Contents.Replace("`", " ` ")}\n```");
			}
		}

		[Command("create"), Aliases("c")]
		public async Task CreateAsync(CommandContext ctx, DiscordUser user, [RemainingText]string note)
		{
			await ctx.Message.DeleteAsync();
			var dbctx = Database.CreateContext();
			if (dbctx.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id))
			{
				await ctx.Member.SendMessageAsync($"**Notes already exist for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`!**");
			}
			else
			{
				dbctx.Modnotes.Add(new DatabaseModNote()
				{
					Contents = note,
					GuildId = (long)ctx.Guild.Id,
					MemberId = (long)user.Id
				});
				await dbctx.SaveChangesAsync();

				await ctx.Member.SendMessageAsync($"**Created ModNote for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}` with content:**\n```\n{note.Replace("`", " ` ")}\n```");
			}
		}

		[Command("clear"), Aliases("clr")]
		public async Task ClearAsync(CommandContext ctx, DiscordUser user)
		{
			await ctx.Message.DeleteAsync();
			var dbctx = Database.CreateContext();
			if(!dbctx.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id))
			{
				await ctx.Member.SendMessageAsync($"**No notes exist for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`!**");
			}
			else
			{
				var old = dbctx.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id);
				dbctx.Modnotes.Remove(old);
				await ctx.Member.SendMessageAsync($"**Cleared ModNote for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`.**");
				await dbctx.SaveChangesAsync();
			}
		}

		[Command("append"), Aliases("a")]
		public async Task AppendAsync(CommandContext ctx, DiscordUser user, [RemainingText]string note)
		{
			await ctx.Message.DeleteAsync();
			var dbctx = Database.CreateContext();
			if (!dbctx.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id))
			{
				await ctx.Member.SendMessageAsync($"**No notes exist for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}`!**");
			}
			else
			{
				var old = dbctx.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)ctx.Guild.Id);
				old.Contents += $"\n{note}";
				dbctx.Modnotes.Update(old);
				await ctx.Member.SendMessageAsync($"**Appended to ModNote for user {user.Username}#{user.Discriminator} in guild `{ctx.Guild.Name}` with new content:**\n```\n{old.Contents.Replace("`", " ` ")}\n```");
				await dbctx.SaveChangesAsync();
			}
		}
	}
}
