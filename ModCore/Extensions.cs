using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using DSharpPlus;

namespace ModCore
{
    public static class Extensions
    {
        /// <summary>
        /// Executes an asynchronous function on the GuildSettings object, that can mutate it.
        /// The resulting GuildSettings will be automatically persisted.
        /// If the guild is not configured, a new GuildSettings object will be created.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="exec">The function to call</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> WithGuildSettings(this CommandContext ctx, Func<GuildSettings, Task> exec)
        {
            var cfg = GetGuildSettings(ctx) ?? new GuildSettings();
            await exec(cfg);
            await ctx.SetGuildSettingsAsync(cfg);
            return ctx;
        }

        /// <summary>
        /// Executes a synchronous function on the GuildSettings object, that can mutate it.
        /// The resulting GuildSettings will be automatically persisted.
        /// If the guild is not configured, a new GuildSettings object will be created.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="exec">The function to call</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> WithGuildSettings(this CommandContext ctx, Action<GuildSettings> exec)
        {
            var cfg = GetGuildSettings(ctx) ?? new GuildSettings();
            exec(cfg);
            await ctx.SetGuildSettingsAsync(cfg);
            return ctx;
        }

        /// <summary>
        /// Executes an asynchronous function <c>there</c> on the GuildSettings object if the guild is configured.
        /// Executes <c>notThere</c> otherwise.
        /// The resulting GuildSettings will be automatically persisted.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="there">The function to call if the guild is configured</param>
        /// <param name="notThere">The function to call if the guild is not configured</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Func<GuildSettings, Task> there, Func<Task> notThere)
        {
            var cfg = GetGuildSettings(ctx);
            if (cfg == null)
            {
                await notThere();
            }
            else
            {
                await there(cfg);
                await ctx.SetGuildSettingsAsync(cfg);
            }
            return ctx;
        }

        /// <summary>
        /// Executes a synchronous function <c>there</c> on the GuildSettings object if the guild is configured.
        /// Executes <c>notThere</c> otherwise.
        /// The resulting GuildSettings will be automatically persisted.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="there">The function to call if the guild is configured</param>
        /// <param name="notThere">The function to call if the guild is not configured</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Action<GuildSettings> there, Action notThere)
        {
            var cfg = GetGuildSettings(ctx);
            if (cfg == null)
            {
                notThere();
            }
            else
            {
                there(cfg);
                await ctx.SetGuildSettingsAsync(cfg);
            }
            return ctx;
        }

        /// <summary>
        /// Executes an asynchronous function <c>there</c> on the GuildSettings object if the guild is configured.
        /// Executes <c>notThere</c> otherwise.
        /// The resulting GuildSettings will be automatically persisted.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="there">The function to call if the guild is configured</param>
        /// <param name="notThere">The function to call if the guild is not configured</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Func<Task> notThere, Func<GuildSettings, Task> there)
        {
            var cfg = GetGuildSettings(ctx);
            if (cfg == null)
            {
                await notThere();
            }
            else
            {
                await there(cfg);
                await ctx.SetGuildSettingsAsync(cfg);
            }
            return ctx;
        }

        /// <summary>
        /// Executes a synchronous function <c>there</c> on the GuildSettings object if the guild is configured.
        /// Executes <c>notThere</c> otherwise.
        /// The resulting GuildSettings will be automatically persisted.
        /// </summary>
        /// <param name="ctx">This object</param>
        /// <param name="there">The function to call if the guild is configured</param>
        /// <param name="notThere">The function to call if the guild is not configured</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Action notThere, Action<GuildSettings> there)
        {
            var cfg = GetGuildSettings(ctx);
            if (cfg == null)
            {
                notThere();
            }
            else
            {
                there(cfg);
                await ctx.SetGuildSettingsAsync(cfg);
            }
            return ctx;
        }

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

        public static async Task LogMessageAsync(this CommandContext ctx, string content = "", DiscordEmbedBuilder embed = null)
        {
            var s = ctx.GetGuildSettings();
            var a = s.ActionLog;
            if (a.Enable)
            {
                var w = await ctx.Client.GetWebhookWithTokenAsync(a.WebhookId, a.WebhookToken);
                await w.ExecuteAsync(content, embeds: embed != null ? new List<DiscordEmbed>() { embed } : null);
            }
        }

        public static async Task ActionLogMessageAsync(this DiscordClient clnt, DiscordGuild gld, DatabaseContext db, string content = "", DiscordEmbedBuilder embed = null)
        {
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == gld.Id);
            var s = cfg.GetSettings();
            if (s.ActionLog.Enable)
            {
                var w = await clnt.GetWebhookWithTokenAsync(s.ActionLog.WebhookId, s.ActionLog.WebhookToken);
                await w.ExecuteAsync(content, embeds: embed != null ? new List<DiscordEmbed>() { embed } : null);
            }
        }

        public static string ToDiscordTag(this DiscordUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }
    }
}
