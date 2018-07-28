using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace ModCore.Logic.Extensions
{
    public static class Sanitization
    {
        // make sure to replace all backslashes preceding @mention so they can't add an extra backslash to escape our escape
        private static readonly Regex EscapeEveryoneMention = new Regex(@"\\*@(everyone|here)");
        
        public static Task<DiscordMessage> SafeRespondAsync(this CommandContext ctx, string s) 
            => ctx.RespondAsync(Sanitize(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> SafeModifyAsync(this CommandContext ctx, DiscordMessage m, string s)
            => m.ModifyAsync(Sanitize(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> SafeMessageAsync(this DiscordChannel channel, string s, bool privileged) 
            => channel.SendMessageAsync(Sanitize(s, privileged));
        
        public static Task<DiscordMessage> SafeMessageAsync(this DiscordChannel channel, string s, CommandContext ctx) 
            => channel.SendMessageAsync(Sanitize(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> SafeRespondAsync(this CommandContext ctx, FormattableString s) 
            => ctx.RespondAsync(SanitizeFormat(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> SafeModifyAsync(this CommandContext ctx, DiscordMessage m, FormattableString s)
            => m.ModifyAsync(SanitizeFormat(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> SafeMessageAsync(this DiscordChannel channel, FormattableString s, bool privileged) 
            => channel.SendMessageAsync(SanitizeFormat(s, privileged));
        
        public static Task<DiscordMessage> SafeMessageAsync(this DiscordChannel channel, FormattableString s, CommandContext ctx) 
            => channel.SendMessageAsync(SanitizeFormat(s, IsPrivileged(ctx)));

        public static Task<DiscordMessage> ElevatedRespondAsync(this CommandContext ctx, string s) 
            => ctx.RespondAsync(s);

        public static Task<DiscordMessage> ElevatedRespondAsync(this CommandContext ctx, DiscordEmbed embed) 
            => ctx.RespondAsync(embed: embed);

        public static Task<DiscordMessage> ElevatedMessageAsync(this DiscordChannel channel, string s) 
            => channel.SendMessageAsync(s);

        public static Task<DiscordMessage> ElevatedMessageAsync(this DiscordChannel channel, DiscordEmbed embed) 
            => channel.SendMessageAsync(embed: embed);

        public static Task<DiscordMessage> ElevatedMessageAsync(this DiscordChannel channel, string s, DiscordEmbed embed) 
            => channel.SendMessageAsync(s, embed: embed);

        public static Task<DiscordMessage> ElevatedMessageAsync(this DiscordMember m, string s) 
            => m.SendMessageAsync(s);

        public static Task<DiscordMessage> ElevatedMessageAsync(this DiscordMember m, DiscordEmbed embed) 
            => m.SendMessageAsync(embed: embed);

        private static string SanitizeFormat(FormattableString s, bool privileged)
        {
            if (privileged)
                return s.ToString(CultureInfo.InvariantCulture);
            var escapedParameters = s.GetArguments()
                .Select<object, object>(e => Sanitize(ToStringInvariant(e), privileged: false));
            return string.Format(s.Format, escapedParameters.ToArray());
        }

        private static string Sanitize(string s, bool privileged)
        {
            return privileged ? s : EscapeEveryoneMention.Replace(s, @"\@$1");
        }
        
        public static bool IsPrivileged(CommandContext ctx) 
            => ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.MentionEveryone);
        
        public static bool IsPrivileged(DiscordMember m, DiscordChannel chan) 
            => m.PermissionsIn(chan).HasPermission(Permissions.MentionEveryone);
        
        private static string ToStringInvariant(object e) => string.Format(CultureInfo.InvariantCulture, "{0}", e);
    }
}