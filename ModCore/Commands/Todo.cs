using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Commands
{
    [Group("todo")]
    public class Todo : BaseCommandModule
    {
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Todo(DatabaseContextBuilder db, InteractivityExtension interactive,
            StartTimes starttimes)
        {
            this.Database = db;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [Command("clear")]
        public async Task ClearAsync(CommandContext context)
        {
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                {
                    var user = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);
                    var data = user.GetData();
                    if(data.TodoItems.Count() == 0)
                    {
                        await context.RespondAsync("Your to do list is empty!");
                        return;
                    }
                    data.TodoItems.Clear();
                    user.SetData(data);
                    db.UserDatas.Update(user);
                    await db.SaveChangesAsync();
                    await context.RespondAsync("Cleared your todo list!");
                }
                else
                {
                    await context.RespondAsync("Your to do list is empty!");
                }
            }
        }

        [Command("check"), Priority(1)]
        public async Task CheckAsync(CommandContext context, int item)
        {
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                {
                    var userdata = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);
                    var data = userdata.GetData();
                    var todo = data.TodoItems;
                    if (todo.Count() < item || item < 1)
                    {
                        await context.RespondAsync("That index doesn't exist!");
                        return;
                    }

                    var listitem = todo[item - 1];

                    listitem.Done = true;
                    todo[item - 1] = listitem;

                    data.TodoItems = todo;
                    userdata.SetData(data);

                    db.UserDatas.Update(userdata);
                    await db.SaveChangesAsync();
                }
                await context.RespondAsync("✅ Checked item on your todo list!");
            }
        }

        [Command("check"), Priority(0)]
        public async Task CheckAsync(CommandContext context, string item)
        {
            int index = 0;
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                {
                    var userdata = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);
                    var data = userdata.GetData();
                    var todo = data.TodoItems;

                    if (todo.Any(x => x.Item.ToLower() == item.ToLower()))
                    {
                        index = todo.IndexOf(todo.First(x => x.Item.ToLower() == item.ToLower()));
                    }
                    else
                    {
                        await context.RespondAsync("⚠️ No such item!");
                        return;
                    }
                }
            }
            await CheckAsync(context, index + 1); // listing starts at 1, array starts at 0. Just add 1
        }

        [Command("remove"), Priority(1)]
        public async Task RemoveAsync(CommandContext context, string item)
        {
            int index = 0;
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                {
                    var userdata = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);
                    var data = userdata.GetData();
                    var todo = data.TodoItems;
                    if(todo.Any(x => x.Item.ToLower() == item.ToLower()))
                    {
                        index = todo.IndexOf(todo.First(x => x.Item.ToLower() == item.ToLower()));
                    }
                    else
                    {
                        await context.RespondAsync("⚠️ No such item!");
                        return;
                    }
                }
            }
            await RemoveAsync(context, index + 1); // listing starts at 1, array starts at 0. Just add 1
        }

        [Command("remove"), Priority(0)]
        public async Task RemoveAsync(CommandContext context, int item)
        {
            using (var db = Database.CreateContext())
            {
                if(db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                {
                    var udata = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);
                    var data = udata.GetData();
                    var todo = data.TodoItems;
                    if(todo.Count() < item || item < 1)
                    {
                        await context.RespondAsync("⚠️ That index doesn't exist!");
                        return;
                    }
                    todo.RemoveAt(item - 1);
                    data.TodoItems = todo;
                    udata.SetData(data);
                    db.UserDatas.Update(udata);
                    await db.SaveChangesAsync();
                }
                await context.RespondAsync("✅ Removed item from your todo list!");
            }
        }

        [Command("add")]
        public async Task AddAsync(CommandContext context, [RemainingText]string item)
        {
            using (var db = Database.CreateContext())
            {
                var userdata = new DatabaseUserData
                {
                    UserId = (long)context.Member.Id
                };
                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                    userdata = db.UserDatas.First(x => x.UserId == (long)context.Member.Id);

                var data = userdata.GetData() ?? new UserData();
                if(data.TodoItems.Any(x => x.Item.ToLower() == item.ToLower()))
                {
                    await context.RespondAsync("⚠️ That item is already in your list!");
                    return;
                }
                data.TodoItems.Add(new TodoItem() { Item = item });
                userdata.SetData(data);

                if (db.UserDatas.Any(x => x.UserId == (long)context.Member.Id))
                    db.UserDatas.Update(userdata);
                else
                    db.UserDatas.Add(userdata);

                await db.SaveChangesAsync();
            }
            await context.RespondAsync("✅ Added item to your todo!");
        }

        [GroupCommand]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            using (var db = Database.CreateContext())
            {
                if (db.UserDatas.Any(x => x.UserId == (long)ctx.Member.Id))
                {
                    var udat = db.UserDatas.Where(x => x.UserId == (long)ctx.Member.Id).First();
                    var todo = udat.GetData().TodoItems;
                    Console.WriteLine(udat.Data);
                    if (!todo.Any())
                    {
                        await ctx.RespondAsync("⚠️ You do not have any todo items yet!");
                        return;
                    }

                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    foreach (var t in todo)
                    {
                        sb.AppendLine($"{i}. {(t.Done ? "\\✅" : "\\❎")} {t.Item}");
                        i++;
                    }

                    var deb = new DiscordEmbedBuilder()
                        .WithTitle("Todo")
                        .WithDescription($"📃 Todo list for {ctx.Member.Username}#{ctx.Member.Discriminator}")
                        .AddField("List", sb.ToString())
                        .Build();

                    await ctx.RespondAsync(embed: deb);
                }
                else
                {
                    await ctx.RespondAsync("⚠️ You do not have any todo items yet!");
                }
            }
        }
    }
}
