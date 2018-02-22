using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;

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
        public static async Task<CommandContext> WithGuildSettings(this CommandContext ctx,
            Func<GuildSettings, Task> exec)
        {
            var cfg = GetGuildSettings(ctx) ?? new GuildSettings();
            await exec(cfg);
            await ctx.SetGuildSettingsAsync(cfg);
            return ctx;
        }

		public static string BreakMentions(this string input)
		{
			input = input.Replace("@", "@\u200B");
			return input;
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
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx,
            Func<GuildSettings, Task> there, Func<Task> notThere)
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
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Action<GuildSettings> there,
            Action notThere)
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
        public static async Task<CommandContext> IfGuildSettingsAsync(this CommandContext ctx, Func<Task> notThere,
            Func<GuildSettings, Task> there)
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
        public static async Task<CommandContext> IfGuildSettings(this CommandContext ctx, Action notThere,
            Action<GuildSettings> there)
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
            var dbb = ctx.Services.GetService<DatabaseContextBuilder>();
            DatabaseGuildConfig cfg = null;
            using (var db = dbb.CreateContext())
                cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id);
            return cfg?.GetSettings();
        }

        public static GuildSettings GetGuildSettings(this DiscordGuild gld, DatabaseContext db)
        {
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == gld.Id);
            return cfg?.GetSettings();
        }

        public static async Task SetGuildSettingsAsync(this CommandContext ctx, GuildSettings gcfg)
        {
            var dbb = ctx.Services.GetService<DatabaseContextBuilder>();
            using (var db = dbb.CreateContext())
            {
                var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id);
                if (cfg == null)
                {
                    cfg = new DatabaseGuildConfig {GuildId = (long) ctx.Guild.Id};
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
        }

        public static async Task LogActionAsync(this CommandContext ctx, string additionalinfo = "")
        {
            var s = ctx.GetGuildSettings() ?? new GuildSettings();
            if (s == null)
                return;
            var a = s.ActionLog;
            var commandArgs = (string.IsNullOrEmpty(ctx.RawArgumentString)) ? "None" : ctx.RawArgumentString;
            if (a.Enable)
            {
                var w = await ctx.Client.GetWebhookWithTokenAsync(a.WebhookId, a.WebhookToken);
                var b = new DiscordEmbedBuilder();

                b.WithTitle($"New Action executed by {ctx.Member.Username}#{ctx.Member.Discriminator}")
                    .WithDescription($"Executed command: {ctx.Command.QualifiedName}\nArguments: {commandArgs}")
                    .WithFooter($"Guild: {ctx.Guild.Name}",
                        string.IsNullOrEmpty(ctx.Guild.IconHash) ? "" : ctx.Guild.IconUrl);
                if (!string.IsNullOrEmpty(additionalinfo))
                    b.AddField("Additional information", additionalinfo);

                var e = new List<DiscordEmbed>()
                {
                    b.Build()
                };

                await w.ExecuteAsync(embeds: e);
            }
        }

        public static async Task LogMessageAsync(this CommandContext ctx, string content = "",
            DiscordEmbedBuilder embed = null)
        {
            var s = ctx.GetGuildSettings();
            var a = s.ActionLog;
            if (a.Enable)
            {
                var w = await ctx.Client.GetWebhookWithTokenAsync(a.WebhookId, a.WebhookToken);
                await w.ExecuteAsync(content, embeds: embed != null ? new List<DiscordEmbed>() {embed} : null);
            }
        }

        public static async Task LogAutoActionAsync(this DiscordClient clnt, DiscordGuild gld, DatabaseContext db,
            string additionalinfo = "")
        {
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == gld.Id);
            var s = cfg.GetSettings();

            if (s.ActionLog.Enable)
            {
                var w = await clnt.GetWebhookWithTokenAsync(s.ActionLog.WebhookId, s.ActionLog.WebhookToken);
                var b = new DiscordEmbedBuilder();

                b.WithTitle($"New Automated action executed")
                    .WithDescription($"Executed action: {additionalinfo}")
                    .WithFooter($"Guild: {gld.Name}", string.IsNullOrEmpty(gld.IconHash) ? "" : gld.IconUrl);

                var e = new List<DiscordEmbed>()
                {
                    b.Build()
                };

                await w.ExecuteAsync(embeds: e);
            }
        }

        public static async Task ActionLogMessageAsync(this DiscordClient clnt, DiscordGuild gld, DatabaseContext db,
            string content = "", DiscordEmbedBuilder embed = null)
        {
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == gld.Id);
            var s = cfg.GetSettings();
            if (s.ActionLog.Enable)
            {
                var w = await clnt.GetWebhookWithTokenAsync(s.ActionLog.WebhookId, s.ActionLog.WebhookToken);
                await w.ExecuteAsync(content, embeds: embed != null ? new List<DiscordEmbed>() {embed} : null);
            }
        }

        public static string ToDiscordTag(this DiscordUser user)
        {
            return $"{user.Username}#{user.Discriminator}";
        }

        public static bool IsNullOrWhitespace(this StringBuilder sb)
        {
            int l;
            if (sb == null || (l = sb.Length) == 0) return true;

            for (var i = 0; i < l; i++)
            {
                if (!char.IsWhiteSpace(sb[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets permission overwrites for this channel as <see cref="Overwrite"/> objects.
        /// </summary>
        /// <param name="channel">the channel to get the overwrites from</param>
        /// <returns>a <see cref="FillingList{Overwrite}"/> object mapping the overwrites</returns>
        public static IReadOnlyList<Overwrite> GetPermissionOverwrites(this DiscordChannel channel)
        {
            return new FillingList<Overwrite>(
                channel.PermissionOverwrites.Select(overwrite => new Overwrite(overwrite, channel.Guild)),
                channel.PermissionOverwrites.Count);
        }

        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }
        
        public static int IndexOfInvariant(this string a, string b)
        {
            return a.IndexOf(b, StringComparison.Ordinal);
        }
    }
}

#region MoreLINQ
#region License and Terms
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace MoreLinq
{
    using System;
    using System.Collections.Generic;

    internal static class MoreEnumerable
    {
        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the default equality comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Returns all distinct elements of the given source, where "distinctness"
        /// is determined via a projection and the specified comparer for the projected type.
        /// </summary>
        /// <remarks>
        /// This operator uses deferred execution and streams the results, although
        /// a set of already-seen keys is retained. If a key is seen multiple times,
        /// only the first element with that key is returned.
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="keySelector">Projection for determining "distinctness"</param>
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
        /// If null, the default equality comparer for <c>TSource</c> is used.</param>
        /// <returns>A sequence consisting of distinct elements from the source sequence,
        /// comparing them by the specified key projection.</returns>

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            
            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }
    }
}
#endregion