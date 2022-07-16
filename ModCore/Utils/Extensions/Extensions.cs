using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Utils;

namespace ModCore.Utils.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Executes an asynchronous function on the GuildSettings object, that can mutate it.
        /// The resulting GuildSettings will be automatically persisted.
        /// If the guild is not configured, a new GuildSettings object will be created.
        /// </summary>
        /// <param name="context">This object</param>
        /// <param name="exec">The function to call</param>
        /// <returns>Asynchronous task resolving to the CommandContext, if you wish to reuse it.</returns>
        public static async Task<CommandContext> WithGuildSettings(this CommandContext context,
            Func<GuildSettings, Task> exec)
        {
            var config = GetGuildSettings(context) ?? new GuildSettings();
            await exec(config);
            await context.SetGuildSettingsAsync(config);
            return context;
        }

        public static async Task<(bool Success, T Result)> RequestArgumentAsync<T>(this CommandContext ctx, string question)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var questionmessage = await ctx.RespondAsync(question);
            var response = await ctx.Message.GetNextMessageAsync();
            await questionmessage.DeleteAsync();


            if (!response.TimedOut)
            {
                if (ctx.Channel.PermissionsFor(ctx.Guild.CurrentMember).HasPermission(Permissions.ManageMessages))
                    await response.Result.DeleteAsync();
                try
                {
                    T result = (T)await ctx.CommandsNext.ConvertArgument<T>(response.Result.Content, ctx);
                    return (true, result);
                }
                catch (Exception)
                {
                    return (false, default(T));
                }
            }

            return (false, default(T));
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

        public static async Task ModLogAsync(this DiscordGuild guild, DatabaseContext db, DiscordEmbedBuilder embed)
        {
            var s = guild.GetGuildSettings(db) ?? new GuildSettings();
            if (s == null)
                return;
            var a = s.Logging;

            if (a.ModLog_Enable)
            {
                var channel = guild.GetChannel(a.ChannelId);

                await channel.SendMessageAsync(embed);
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

        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }
        
        public static int IndexOfInvariant(this string a, string b)
        {
            return a.IndexOf(b, StringComparison.Ordinal);
        }
        
        /// <summary>
        /// Checks if a given member can interact with another member (kick, ban, modify permissions).
        /// Note that this only checks the role position and not the actual permission.
        /// </summary>
        /// <param name="this">this object</param>
        /// <param name="target">the member to check against</param>
        /// <returns>true if this member can interact with the target</returns>
        /// <exception cref="ArgumentNullException">if target is null</exception>
        public static bool CanInteract(this DiscordMember @this, DiscordMember target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var guild = @this.Guild;
            if (guild != target.Guild)
            {
                throw new ArgumentException("Provided members must both be Member objects of the same Guild!",
                    nameof(target));
            }

            if (guild.Owner == @this) return true;
            if (guild.Owner == target) return false;

            var issuerRole = @this.Roles.FirstOrDefault();
            var targetRole = target.Roles.FirstOrDefault();
            return issuerRole != null && (targetRole == null || issuerRole.CanInteract(targetRole));
        }
        
        /// <summary>
        /// Checks if a given role can interact with another role (kick, ban, modify permissions).
        /// Note that this only checks the role position and not the actual permission.
        /// </summary>
        /// <param name="this">this object</param>
        /// <param name="target">the role to check against</param>
        /// <returns>true if this role can interact with the target</returns>
        /// <exception cref="ArgumentNullException">if target is null</exception>
        public static bool CanInteract(this DiscordRole @this, DiscordRole target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            
            return target.Position < @this.Position;
        }

        public static async Task<(bool TimedOut, bool Accepted, DiscordMessage FollowupMessage)> ConfirmAsync(this BaseContext ctx, 
            DiscordFollowupMessageBuilder message, string confirmText, string denyText,
            InteractivityExtension interactivity, DiscordEmoji confirmEmoji = null, DiscordEmoji DenyEmoji = null)
        {
            var confirmationId = Math.Abs(new Random().Next()).ToString();

            var accept = new DiscordButtonComponent(ButtonStyle.Danger, confirmationId + "confirm", confirmText, false,
                confirmEmoji != null ? new DiscordComponentEmoji(confirmEmoji) : null);
            var deny = new DiscordButtonComponent(ButtonStyle.Secondary, confirmationId + "deny", denyText, false,
                DenyEmoji != null ? new DiscordComponentEmoji(DenyEmoji) : null);

            message.AddComponents(accept, deny);
            await ctx.DeferAsync(true);
            var msg = await ctx.FollowUpAsync(message);

            var confirmation = await interactivity.WaitForEventArgsAsync<ComponentInteractionCreateEventArgs>(
                x => x.Interaction.Data.ComponentType == ComponentType.Button 
                && x.Interaction.Data.CustomId.StartsWith(confirmationId)
                && x.User.Id == ctx.User.Id);

            if (confirmation.TimedOut)
                return (true, false, msg);

            var confirmed = confirmation.Result.Interaction.Data.CustomId.EndsWith("confirm");

            return (false, confirmed, msg);
        }

        public static async Task<(bool TimedOut, T choice, DiscordMessage FollowupMessage)> MakeChoiceAsync<T>(this BaseContext ctx,
            DiscordFollowupMessageBuilder message, IDictionary<string, T> choices, InteractivityExtension interactivity)
        {
            var choiceId = Math.Abs(new Random().Next()).ToString();
            List<DiscordSelectComponentOption> options = new List<DiscordSelectComponentOption>();

            foreach(var choice in choices)
            {
                options.Add(new DiscordSelectComponentOption(choice.Key, choice.Key));
            }

            var select = new DiscordSelectComponent(choiceId, choices.First().Key, options);

            message.AddComponents(select);

            var msg = await ctx.FollowUpAsync(message);

            var confirmation = await interactivity.WaitForEventArgsAsync<ComponentInteractionCreateEventArgs>(
                x => x.Interaction.Data.ComponentType == ComponentType.Select
                && x.Values.Any(x => choices.Any(y => y.Key == x))
                && x.User.Id == ctx.User.Id);

            if (confirmation.TimedOut)
                return (true, default(T), msg);

            return (false, choices.First(x => x.Key == confirmation.Result.Values[0]).Value, msg);
        }
    }
}