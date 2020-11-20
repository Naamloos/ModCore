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

	    [Command("dbtest"), Hidden]
	    public async Task DbTestAsync(CommandContext ctx, [RemainingText] string s)
	    {
	        if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
	        {
	            await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
	            return;
	        }

	        using (var db = this.Database.CreateContext())
	        {
	            await db.CommandIds.AddAsync(new DatabaseCommandId()
	            {
	                Command = "test" + s
	            });
	            await ctx.SafeRespondUnformattedAsync("a"+string.Join(",", db.CommandIds.Select(e => e.Command + ":" + e.Id)));
	            await db.SaveChangesAsync();
	            await ctx.SafeRespondUnformattedAsync("b"+string.Join(",", db.CommandIds.Select(e => e.Command + ":" + e.Id)));
	        }
	    }

	    [Command("exit"), Aliases("e"), Hidden]
        public async Task ExitAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
            {
                await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
                return;
            }

            await ctx.SafeRespondUnformattedAsync("Are you sure you want to shut down the bot?");

            var cts = ctx.Services.GetService<SharedData>().CTS;
            var interactivity = ctx.Services.GetService<InteractivityExtension>();
            var m = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m.TimedOut)
            {
                await ctx.SafeRespondUnformattedAsync("Timed out.");
            }
            else if (InteractivityUtil.Confirm(m.Result))
            {
                await ctx.SafeRespondUnformattedAsync("Shutting down.");
                cts.Cancel(false);
            }
            else
            {
                await ctx.SafeRespondUnformattedAsync("Operation cancelled by user.");
            }
        }

		[Command("apitoken"), Hidden]
		public async Task ApiToken(CommandContext ctx)
		{
			if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
			{
				await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
				return;
			}
			int tk = new Random().Next(0, int.MaxValue);
			await ctx.RespondAsync("Received a new token by DM!");
			this.Shared.ModCore.SharedData.ApiToken = tk.ToString();
			await ctx.Member.SendMessageAsync(tk.ToString());
		}

        [Command("testupdate"), Aliases("t"), Hidden]
        public async Task ThrowAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
            {
                await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
                return;
            }
            await ctx.SafeRespondUnformattedAsync("Test: 420");
        }

        [Command("sudo"), Aliases("s"), Hidden]
        public async Task SudoAsync(CommandContext ctx, [Description("Member to sudo")]DiscordMember m, [Description("Command to sudo"), RemainingText]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
            {
                await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
                return;
            }

            var cmdobj = ctx.CommandsNext.FindCommand(command, out string args);
            var p = ctx.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fctx = ctx.CommandsNext.CreateFakeContext(m, ctx.Channel, command, p, cmdobj);
            await ctx.CommandsNext.ExecuteCommandAsync(fctx);
        }

        [Command("sudoowner"), Aliases("so"), Hidden]
        public async Task SudoOwnerAsync(CommandContext ctx, [RemainingText, Description("Command to sudo")]string command)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
            {
                await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
                return;
            }

            var cmdobj = ctx.CommandsNext.FindCommand(command, out string args);
            var p = ctx.GetGuildSettings()?.Prefix ?? this.Shared.DefaultPrefix;
            var fctx = ctx.CommandsNext.CreateFakeContext(ctx.Guild.Owner, ctx.Channel, command, p, cmdobj);
            await ctx.CommandsNext.ExecuteCommandAsync(fctx);
        }

        //[Command("update"), Aliases("u"), Hidden]
        //public async Task UpdateAsync(CommandContext ctx)
        //{
        //    if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
        //    {
        //        await ctx.SafeRespondAsync("You do not have permission to use this command!");
        //        return;
        //    }
        //    var m = await ctx.SafeRespondAsync($"Running update script...");
        //    const string fn = "update";
        //    if (File.Exists("update.sh"))
        //    {
        //        const string file = fn + ".sh";
        //        var proc = new Process
        //        {
        //            StartInfo = new ProcessStartInfo
        //            {
        //                FileName = "nohup",
        //                Arguments = $"bash {file} {Process.GetCurrentProcess().Id} {ctx.Guild.Id} {ctx.Channel.Id}",
        //                UseShellExecute = false,
        //                RedirectStandardOutput = true,
        //                CreateNoWindow = true
        //            }
        //        };
        //        proc.Start();
        //        await m.ModifyAsync($"Updating ModCore using `{file}`. See you soon!");
        //    }
        //    else if (File.Exists("update.bat"))
        //    {
        //        const string file = fn + ".bat";
        //        var proc = new Process
        //        {
        //            StartInfo = new ProcessStartInfo
        //            {
        //                FileName = file,
        //                Arguments = $"{ctx.Guild.Id} {ctx.Channel.Id}",
        //                UseShellExecute = false,
        //                RedirectStandardOutput = true,
        //                CreateNoWindow = true
        //            }
        //        };
        //        proc.Start();
        //        await m.ModifyAsync($"Updating ModCore using `{file}`. See you soon!");
        //    }
        //    else
        //    {
        //        await m.ModifyAsync("**‼ Your update script has not been found. ‼**\n\nPlease place `update.sh` (Linux) or `update.bat` (Windows) in your ModCore directory.");
        //        return;
        //    }
        //    this.Shared.CTS.Cancel();
        //}

        [Command("botmanagers"), Aliases("bm"), Hidden]
        public async Task BotManagersAsync(CommandContext ctx)
        {
            if (!Shared.BotManagers.Contains(ctx.Member.Id) && !ctx.Client.CurrentApplication.Owners.Any(x => x.Id == ctx.User.Id))
            {
                await ctx.SafeRespondUnformattedAsync("You do not have permission to use this command!");
                return;
            }
            var list = new List<string>();
            foreach (ulong manager in Shared.BotManagers)
            {
                try
                {
                    DiscordMember m = await ctx.Guild.GetMemberAsync(manager);
                    list.Add(m.DisplayName);
                }
                catch
                {

                }
            }
            await ctx.SafeRespondAsync($"Users with access: {(list.Count > 0 ? $"`{string.Join("`, `", list)}`" : "None")}");
        }

        [Command("grantxp"), Aliases("gxp"), Hidden]
        public async Task GrantXpAsync(CommandContext ctx, DiscordMember m, int xp)
        {
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)m.Id))
                {
                    var dat = db.UserDatas.First(x => x.UserId == (long)m.Id);
                    var data = dat.GetData();

                    if (data.ServerExperience.ContainsKey(ctx.Guild.Id))
                    {
                        data.ServerExperience[ctx.Guild.Id] += xp;
                        await ctx.RespondAsync($"Granted {xp} xp.");
                    }
                    else
                    {
                        await ctx.RespondAsync("No xp data stored for this user/guild combo");
                        return;
                    }

                    dat.SetData(data);
                    db.UserDatas.Update(dat);

                    await db.SaveChangesAsync();
                }
            }
        }

        [Command("query"), Aliases("q"), Hidden]
        public async Task QueryAsync(CommandContext ctx, string table, [RemainingText] string query)
        {
            using (var db = Database.CreateContext())
            {
                var obj = new BoxingList(await QueryTableAsync(table, query, db));
	            await ctx.ElevatedRespondAsync($@"
{obj.Count} results of query onto {table}:
{obj.Aggregate((current, next) => $"{current}, {next}")}
");
            }
        }

	    private static async Task<IList> QueryTableAsync(string table, string query, DatabaseContext db)
	    {
	        switch (table.ToUpperInvariant())
	        {
	            case "DATABASEINFO": return await QueryAsync(db.Info, query);

                case "GUILDCONFIG":
	            case "DATABASEGUILDCONFIG": return await QueryAsync(db.GuildConfig, query);

                case "MODNOTE":
	            case "NOTE":
	            case "DATABASEMODNOTE": return await QueryAsync(db.Modnotes, query);

                case "ROLESTATEOVERRIDE":
	            case "DATABASEROLESTATEOVERRIDE":
	            case "ROLESTATEROLES":
	            case "DATABASEROLESTATEROLES": return await QueryAsync(db.RolestateOverrides, query);

                case "TIMER":
	            case "DATABASETIMER": return await QueryAsync(db.Timers, query);

                case "STARDATA":
	            case "STAR": 
	            case "DATABASESTARDATA": return await QueryAsync(db.StarDatas, query);

                case "TAG": 
	            case "DATABASETAG": return await QueryAsync(db.Tags, query);
                case "COMMANDID": return await QueryAsync(db.CommandIds, query);
	            default: throw new ArgumentException();
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
