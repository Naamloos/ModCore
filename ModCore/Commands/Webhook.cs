using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [Group("webhook"), CheckDisable]
    public class Webhook : BaseCommandModule
    {
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Webhook(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive,
            StartTimes starttimes)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [Command("resettoken")]
        public async Task ResetTokenAsync(CommandContext ctx)
        {
            using (var db = Database.CreateContext())
            {
                var udata = new DatabaseUserData
                {
                    UserId = (long)ctx.Member.Id
                };
                if (db.UserDatas.Any(x => x.UserId == (long)ctx.Member.Id))
                    udata = db.UserDatas.First(x => x.UserId == (long)ctx.Member.Id);

                var data = udata.GetData() ?? new UserData();
                // pre userdata action

                data.Token = GenerateNewToken();

                await ctx.RespondAsync("Invalidated your user token and generated a new one. All your existing webhooks stop working!");

                // post userdata action
                udata.SetData(data);

                if (db.UserDatas.Any(x => x.UserId == (long)ctx.Member.Id))
                    db.UserDatas.Update(udata);
                else
                    db.UserDatas.Add(udata);

                await db.SaveChangesAsync();
            }
        }

        [Command("get"), RequireUserPermissions(Permissions.ManageMessages | Permissions.ManageChannels)]
        public async Task GetWebhookAsync(CommandContext ctx, DiscordChannel c)
        {
            using (var db = Database.CreateContext())
            {
                var udata = new DatabaseUserData
                {
                    UserId = (long)ctx.Member.Id
                };
                if (db.UserDatas.Any(x => x.UserId == (long)ctx.Member.Id))
                    udata = db.UserDatas.First(x => x.UserId == (long)ctx.Member.Id);

                var data = udata.GetData() ?? new UserData();
                // pre userdata action

                if (string.IsNullOrEmpty(data.Token))
                {
                    data.Token = GenerateNewToken();
                }

                await ctx.RespondAsync("Webhook link sent by DM.");
                await ctx.Member.SendMessageAsync($"Your webhook is https://modcore.naamloos.dev/api/webhook/{ctx.User.Id}/{ctx.Channel.Id}/{data.Token}" +
                    $"\nCommands will be executed as if they are executed by the member that owns this token.\n" +
                    $"**Do NOT share this link with anyone, especially your token!! (the last gibberish text)**\n\n" +
                    $"Setup + Advanced guide: https://github.com/Naamloos/ModCore/wiki/Webhook---IFTTT-Guide");

                // post userdata action
                udata.SetData(data);

                if (db.UserDatas.Any(x => x.UserId == (long)ctx.Member.Id))
                    db.UserDatas.Update(udata);
                else
                    db.UserDatas.Add(udata);

                await db.SaveChangesAsync();
            }
        }

        [Command("get"), RequireUserPermissions(Permissions.ManageMessages | Permissions.ManageChannels)]
        public async Task GetWebhookAsync(CommandContext ctx)
        {
            await this.GetWebhookAsync(ctx, ctx.Channel);
        }

        const string CHARBAG = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.!@*+=-";
        const int TOKEN_LEN = 20;
        private string GenerateNewToken()
        {
            string token = "";
            Random r = new Random();

            for (int i = 0; i < TOKEN_LEN; i++)
                token += CHARBAG[r.Next(0, CHARBAG.Length)];

            return token;
        }
    }
}
