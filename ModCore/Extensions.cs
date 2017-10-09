using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;

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

        public static string ToDiscordTag(this DiscordUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }
    }
}
