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
		public async Task ExecuteGroupAsync(CommandContext context, [Description("Member to get notes about.")]DiscordUser user)
		{
			await context.Message.DeleteAsync();
			var db = Database.CreateContext();
			
			if (!db.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id))
			{
				await context.Member.SendMessageAsync($"⚠️ No notes exist for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`!");
			}
			else
			{
				var note = db.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id);
				await context.Member.SendMessageAsync($"📃 Notes for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`:\n```\n{note.Contents.Replace("`", " ` ")}\n```");
			}
		}

		[Command("create"), Aliases("c")]
		public async Task CreateAsync(CommandContext context, DiscordUser user, [RemainingText]string note)
		{
			await context.Message.DeleteAsync();
			var db = Database.CreateContext();
			if (db.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id))
			{
				await context.Member.SendMessageAsync($"⚠️ Notes already exist for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`!");
			}
			else
			{
				db.Modnotes.Add(new DatabaseModNote()
				{
					Contents = note,
					GuildId = (long)context.Guild.Id,
					MemberId = (long)user.Id
				});
				await db.SaveChangesAsync();

				await context.Member.SendMessageAsync($"✅ Created ModNote for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}` with content:\n```\n{note.Replace("`", " ` ")}\n```");
			}
		}

		[Command("clear"), Aliases("clr")]
		public async Task ClearAsync(CommandContext context, DiscordUser user)
		{
			await context.Message.DeleteAsync();
			var db = Database.CreateContext();
			if(!db.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id))
			{
				await context.Member.SendMessageAsync($"⚠️ No notes exist for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`!");
			}
			else
			{
				var old = db.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id);
				db.Modnotes.Remove(old);
				await context.Member.SendMessageAsync($"✅ Cleared ModNote for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`.");
				await db.SaveChangesAsync();
			}
		}

		[Command("append"), Aliases("a")]
		public async Task AppendAsync(CommandContext context, DiscordUser user, [RemainingText]string note)
		{
			await context.Message.DeleteAsync();
			var db = Database.CreateContext();
			if (!db.Modnotes.Any(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id))
			{
				await context.Member.SendMessageAsync($"⚠️ No notes exist for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}`!");
			}
			else
			{
				var old = db.Modnotes?.First(x => x.MemberId == (long)user.Id && x.GuildId == (long)context.Guild.Id);
				old.Contents += $"\n{note}";
				db.Modnotes.Update(old);
				await context.Member.SendMessageAsync($"✅ Appended to ModNote for user {user.Username}#{user.Discriminator} in guild `{context.Guild.Name}` with new content: \n```\n{old.Contents.Replace("`", " ` ")}\n```");
				await db.SaveChangesAsync();
			}
		}
	}
}
