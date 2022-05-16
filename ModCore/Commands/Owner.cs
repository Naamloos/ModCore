using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("owner"), Aliases("o"), Hidden]
    public class Owner : BaseCommandModule
	{
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }

        public Owner(SharedData shared, DatabaseContextBuilder db)
        {
            this.Shared = shared;
            this.Database = db;
        }

	    [Command("exit"), Aliases("e"), Hidden]
        public async Task ExitAsync(CommandContext context)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            await context.SafeRespondUnformattedAsync("❓ Are you sure you want to shut down the bot?");

            var cancellationtokensource = context.Services.GetService<SharedData>().CancellationTokenSource;
            var interactivity = context.Services.GetService<InteractivityExtension>();
            var message = await interactivity.WaitForMessageAsync(x => x.ChannelId == context.Channel.Id && x.Author.Id == context.Member.Id, TimeSpan.FromSeconds(30));

            if (message.TimedOut)
            {
                await context.SafeRespondUnformattedAsync("⚠️⌛ Timed out.");
            }
            else if (InteractivityUtil.Confirm(message.Result))
            {
                await context.SafeRespondUnformattedAsync("✅ Shutting down.");
                cancellationtokensource.Cancel(false);
            }
            else
            {
                await context.SafeRespondUnformattedAsync("✅ Operation cancelled by user.");
            }
        }

        [Command("sudo"), Aliases("s"), Hidden]
        public async Task SudoAsync(CommandContext context, [Description("Member to sudo")]DiscordMember member, [Description("Command to sudo"), RemainingText]string command)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            var commandobject = context.CommandsNext.FindCommand(command, out string args);
            var prefix = context.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fakecontext = context.CommandsNext.CreateFakeContext(member, context.Channel, command, prefix, commandobject, args);
            await context.CommandsNext.ExecuteCommandAsync(fakecontext);
        }

        [Command("sudoowner"), Aliases("so"), Hidden]
        public async Task SudoOwnerAsync(CommandContext context, [RemainingText, Description("Command to sudo")]string command)
        {
            if (!Shared.BotManagers.Contains(context.Member.Id) && !context.Client.CurrentApplication.Owners.Any(x => x.Id == context.User.Id))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You do not have permission to use this command!");
                return;
            }

            var commandobject = context.CommandsNext.FindCommand(command, out string args);
            var prefix = context.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fakecontext = context.CommandsNext.CreateFakeContext(context.Guild.Owner, context.Channel, command, prefix, commandobject, args);
            await context.CommandsNext.ExecuteCommandAsync(fakecontext);
        }

        [Command("grantxp"), Aliases("gxp"), Hidden]
        public async Task GrantXpAsync(CommandContext context, DiscordMember member, int experience)
        {
            using (var db = Database.CreateContext())
            {
                var data = db.Levels.FirstOrDefault(x => x.UserId == (long)member.Id && x.GuildId == (long)context.Guild.Id);

                if (data != null)
                {
                    data.Experience += experience;
                    await context.RespondAsync($"✅ Granted {experience} xp to {member.DisplayName}.");
                    db.Levels.Update(data);

                    await db.SaveChangesAsync();
                }
                else
                {
                    await context.RespondAsync("⚠️ No xp data stored for this user/guild combo");
                    return;
                }
            }
        }

        [Command("query"), Aliases("q"), Hidden]
        public async Task QueryAsync(CommandContext context, string table, [RemainingText] string query)
        {
            using (var db = Database.CreateContext())
            {
                var obj = new BoxingList(await QueryTableAsync(table, query, db));
	            await context.ElevatedRespondAsync($@"✅
{obj.Count} results of query onto {table}:
{obj.Aggregate((current, next) => $"{current}, {next}")}
");
            }
        }

	    private static async Task<IList> QueryTableAsync(string table, string query, DatabaseContext db)
	    {
	        switch (table.ToUpperInvariant())
	        {
	            case "DATABASEINFO":
                    return await QueryAsync(db.Info, query);

                case "GUILDCONFIG":
	            case "DATABASEGUILDCONFIG":
                    return await QueryAsync(db.GuildConfig, query);

                case "MODNOTE":
	            case "NOTE":
	            case "DATABASEMODNOTE":
                    return await QueryAsync(db.Modnotes, query);

                case "ROLESTATEOVERRIDE":
	            case "DATABASEROLESTATEOVERRIDE":
                    return await QueryAsync(db.RolestateOverrides, query);

                case "ROLESTATEROLES":
	            case "DATABASEROLESTATEROLES":
	                return await QueryAsync(db.RolestateRoles, query);

	            case "TIMER":
	            case "DATABASETIMER":
                    return await QueryAsync(db.Timers, query);

                case "STARDATA":
	            case "STAR":
	            case "DATABASESTARDATA":
	                return await QueryAsync(db.StarDatas, query);

	            case "TAG": 
	            case "DATABASETAG":
                    return await QueryAsync(db.Tags, query);

                case "COMMANDID":
                    return await QueryAsync(db.CommandIds, query);

                default: 
                    throw new ArgumentException();
	        }
	    }

		private static Task<List<T>> QueryAsync<T>(DbSet<T> set, string query) where T : class 
			=> set.FromSqlRaw(query).ToListAsync();
		
		private class BoxingList : IList, IList<object>, IReadOnlyList<object>
		{
			private readonly IList _inner;

			public BoxingList(IList list) => _inner = list;

			IEnumerator<object> IEnumerable<object>.GetEnumerator() => _inner.Cast<object>().GetEnumerator();
			bool ICollection<object>.Remove(object item)
			{
				if (!_inner.Contains(item)) return false;
				_inner.Remove(item);
				return true;
			}

			public IEnumerator GetEnumerator() => _inner.GetEnumerator();
			public void CopyTo(Array array, int index) => _inner.CopyTo(array, index);
			public int Count => _inner.Count;
			public bool IsSynchronized => _inner.IsSynchronized;
			public object SyncRoot => _inner.SyncRoot;
			public int Add(object value) => _inner.Add(value);
			void ICollection<object>.Add(object item) => _inner.Add(item);
			public void Clear() => _inner.Clear();
			public bool Contains(object value) => _inner.Contains(value);
			public void CopyTo(object[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
			public int IndexOf(object value) => _inner.IndexOf(value);
			public void Insert(int index, object value) => _inner.Insert(index, value);
			public void Remove(object value) => _inner.Remove(value);
			public void RemoveAt(int index) => _inner.RemoveAt(index);
			public bool IsFixedSize => _inner.IsFixedSize;
			public bool IsReadOnly => _inner.IsReadOnly;

			public object this[int index]
			{
				get => _inner[index];
				set => _inner[index] = value;
			}

			public override string ToString() => _inner.ToString();
			public override int GetHashCode() => _inner.GetHashCode();
		}
	}
}
