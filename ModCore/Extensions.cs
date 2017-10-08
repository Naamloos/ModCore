using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using System.Collections.Generic;

namespace ModCore
{
    public static class Extensions
    {
        public static GuildSettings GetGuildSettings(this CommandContext ctx)
        {
            var dbb = ctx.Dependencies.GetDependency<DatabaseContextBuilder>();
            var db = dbb.CreateContext();
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == ctx.Guild.Id);
            return cfg?.GetSettings();
        }

        public static GuildSettings GetGuildSettings(this DiscordGuild gld, DatabaseContext db)
        {
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == gld.Id);
            return cfg?.GetSettings();
        }

        public static async Task SetGuildSettingsAsync(this CommandContext ctx, GuildSettings gcfg)
        {
            var dbb = ctx.Dependencies.GetDependency<DatabaseContextBuilder>();
            var db = dbb.CreateContext();
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == ctx.Guild.Id);
            if (cfg == null)
            {
                cfg = new DatabaseGuildConfig { GuildId = (long)ctx.Guild.Id };
                cfg.SetSettings(gcfg);
                await db.GuildConfig.AddAsync(cfg);
            }
            else
            {
                cfg.SetSettings(gcfg);
                db.GuildConfig.Update(cfg);
            }

            await db.SaveChangesAsync();
        }

        public static async Task LogAction(this CommandContext ctx, string additionalinfo = "")
        {
            var s = ctx.GetGuildSettings();
            var a = s.ActionLog;
            var commandArgs = (string.IsNullOrEmpty(ctx.RawArgumentString)) ? "None" : ctx.RawArgumentString;
            if (a.Enable)
            {
                var w = await ctx.Client.GetWebhookWithTokenAsync(a.WebhookId, a.WebhookToken);
                var b = new DiscordEmbedBuilder();

                b.WithTitle($"New Action executed by {ctx.Member.Username}#{ctx.Member.Discriminator}")
                    .WithDescription($"Executed command: {ctx.Command.QualifiedName}\nArguments: {commandArgs}")
                    .WithFooter($"Guild: {ctx.Guild.Name}", string.IsNullOrEmpty(ctx.Guild.IconHash) ? "" : ctx.Guild.IconUrl);
                if (!string.IsNullOrEmpty(additionalinfo))
                    b.AddField("Additional information", additionalinfo);

                var e = new List<DiscordEmbed>()
                {
                    b.Build()
                };

                await w.ExecuteAsync(embeds: e);
            }
        }
    }
}
