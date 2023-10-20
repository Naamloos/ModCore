using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
    public class Reactions
    {
        [AsyncListener(EventType.MessageReactionAdded)]
        public static async Task ReactionAdd(MessageReactionAddEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            GuildSettings config = null;
            using (var db = database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

                // Reaction roles
				if (config.ReactionRoles.Any(x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id 
                    && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name))
				{
					var reactionroleid = (ulong)config.ReactionRoles.First(
						x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id 
                            && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name).RoleId;
					var reactionrole = eventargs.Channel.Guild.GetRole(reactionroleid);
					var member = await eventargs.Channel.Guild.GetMemberAsync(eventargs.User.Id);
					if(!member.Roles.Any(x => x.Id == reactionroleid))
						await member.GrantRoleAsync(reactionrole);
				}

                return;
            }
        }

        [AsyncListener(EventType.MessageReactionRemoved)]
        public static async Task ReactionRemove(MessageReactionRemoveEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            GuildSettings config = null;
            using (var db = database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

				if (config.ReactionRoles.Any(x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name))
				{
					var reactionroleid = (ulong)config.ReactionRoles.First(
						x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name).RoleId;
					var reactionrole = eventargs.Channel.Guild.GetRole(reactionroleid);
					var member = await eventargs.Channel.Guild.GetMemberAsync(eventargs.User.Id);
					if (member.Roles.Any(x => x.Id == reactionroleid))
						await member.RevokeRoleAsync(reactionrole);
				}

                return;
            }
        }
    }
}
