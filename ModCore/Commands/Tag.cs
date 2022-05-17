using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("tag"), CheckDisable]
    public class Tag : BaseCommandModule
	{
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Tag(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive,
            StartTimes starttimes)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [Priority(0)]
        [GroupCommand]
		public async Task ExecuteGroupAsync(CommandContext context, 
            [RemainingText, Description("Tag to get information about.")] string name)
        {
            if(tryGetTag(name, context.Channel, out DatabaseTag tag))
            {
                await context.RespondAsync($"🏷 `{name}`:\n\n{tag.Contents}");
                return;
            }

            await context.SafeRespondAsync($"⚠️ No such tag exists!");
        }

        [Priority(1)]
        [Command("set"), CheckDisable]
        [Description("Sets a new tag for this channel, or modifies one if you own it.")]
        public async Task SetAsync(CommandContext context, [Description("Tag to create")]string name, [Description("Contents of tag"), RemainingText]string contents)
        {
            bool isNew = false;
            DatabaseTag tag;

            if(tryGetTag(name, context.Channel, out tag))
            {
                if (!canManageTag(tag, context.Channel, context.Member))
                {
                    await context.SafeRespondUnformattedAsync("⚠️ That tag already exists and you don't own it!");
                    return;
                }
            }
            else
            {
                tag = new DatabaseTag
                {
                    ChannelId = -1, // guild tag
                    Name = name,
                    GuildId = (long)context.Guild.Id,
                    OwnerId = (long)context.Member.Id,
                    CreatedAt = DateTime.Now
                };
                isNew = true;
            }

            using (var db = this.Database.CreateContext())
            {
                tag.Contents = contents;
                if(isNew)
                {
                    db.Tags.Add(tag);
                    await context.SafeRespondAsync($"✅ Tag `{name.BreakMentions()}` succesfully created!");
                }
                else
                {
                    db.Tags.Update(tag);
                    await context.SafeRespondAsync($"✅ Tag `{name.BreakMentions()}` succesfully modified!");
                }
                await db.SaveChangesAsync();
            }
        }

        [Priority(1)]
        [Command("override"), CheckDisable]
        [Description("Overrides a tag for a specific channel")]
        public async Task OverrideAsync(CommandContext context, [Description("Override tag to create")] string name, [Description("Contents of tag"), RemainingText] string contents)
        {
            DatabaseTag tag;
            bool isNew = false;
            if (!tryGetTag(name, context.Channel, out tag)
                || tag.ChannelId < 1)
            {
                tag = new DatabaseTag()
                {
                    ChannelId = (long)context.Channel.Id,
                    GuildId = (long)context.Guild.Id,
                    Contents = contents,
                    CreatedAt = DateTime.Now,
                    Name = name,
                    OwnerId = (long)context.User.Id
                };
                isNew = true;
            }
            else if(!canManageTag(tag, context.Channel, context.Member))
            {
                await context.SafeRespondUnformattedAsync("⚠️ That tag already exists and you don't own it!");
                return;
            }

            using (var db = this.Database.CreateContext())
            {
                tag.Contents = contents;
                if (isNew)
                {
                    db.Tags.Add(tag);
                    await context.SafeRespondAsync($"✅ Channel override tag `{name.BreakMentions()}` succesfully created!");
                }
                else
                {
                    db.Tags.Update(tag);
                    await context.SafeRespondAsync($"✅ Channel override tag `{name.BreakMentions()}` succesfully modified!");
                }
                await db.SaveChangesAsync();
            }
        }

        [Priority(1)]
        [Command("remove"), Aliases("delete", "gonezo"), CheckDisable]
        [Description("Removes a tag, if it exists and you own it")]
        public async Task RemoveAsync(CommandContext context, [Description("Tag to remove"), RemainingText]string name)
        {
            if(!tryGetTag(name, context.Channel, out DatabaseTag tag))
            {
                await context.SafeRespondAsync($"⚠️ No such tag exists!");
                return;
            }

            if(!canManageTag(tag, context.Channel, context.Member))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You don't own that tag!");
                return;
            }

            await context.RespondAsync($"❗ Do you really want to delete " +
                $"{(tag.ChannelId == 0? "server" : "channel")} tag `{name.BreakMentions()}`?");

            bool y = false;
            var response 
                = await Interactivity.WaitForMessageAsync(x => AugmentedBoolConverter.TryConvert(x.Content, context, out y) ? y : false);

            if(!response.TimedOut && y)
            {
                using (var db = this.Database.CreateContext())
                {
                    db.Tags.Remove(tag);
                    await db.SaveChangesAsync();
                    await context.SafeRespondAsync($"✅ Tag `{name.BreakMentions()}` successfully removed!");
                }
                return;
            }

            await context.SafeRespondAsync($"⚠️ Canceled deleting tag `{name.BreakMentions()}`");
        }

        [Priority(1)]
        [Command("info"), CheckDisable]
        [Description("Shows info about a tag.")]
        public async Task InfoAsync(CommandContext context, [Description("Tag to show information about"), RemainingText] string name)
        {
            if(!tryGetTag(name, context.Channel, out DatabaseTag tag))
            {
                await context.SafeRespondAsync($"⚠️ No such tag exists!");
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"🏷 {name}")
                .WithDescription($"Created at: {tag.CreatedAt.ToString()}\nOwned by: <@{tag.OwnerId.ToString()}>")
                .AddField("Tag Type", tag.ChannelId == 0? "This tag is a server-global tag." : "This tag is a channel-specific override.")
                .AddField("Content", tag.Contents);

            await context.ElevatedRespondAsync(embed: embed);
        }

        [Priority(1)]
        [Command("list"), CheckDisable]
        [Description("Lists tags for this channel.")]
        public async Task ListAsync(CommandContext ctx)
        {
            using (var db = this.Database.CreateContext())
            {
                var list = db.Tags.Where(x => (x.GuildId == (long)ctx.Guild.Id && x.ChannelId < 1)).ToList();
                var channelist = db.Tags.Where(x => x.ChannelId == (long)ctx.Channel.Id).ToList();
                list = list.Where(x => !channelist.Any(y => y.Name == x.Name)).ToList();
                list.AddRange(channelist);
                list.OrderByDescending(x => x.Name);

                if (list.Count() < 1)
                {
                    await ctx.SafeRespondUnformattedAsync("⚠️ This channel has no tags!");
                }
                else
                {
                    string tags = string.Join("\n", list.Select(x => x.ChannelId < 1? $"🌍 `{x.Name}`" : $"💬 `{x.Name}`"));
                    var embedBase = new DiscordEmbedBuilder()
                        .WithTitle("🏷 Tags available in this channel");

                    var pages = this.Interactivity.GeneratePagesInEmbed(tags, SplitType.Line, embedBase);
                    await this.Interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages, new PaginationEmojis());
                }
            }
        }

        [Priority(1)]
        [Command("transfer"), CheckDisable]
        [Description("Transfers ownership of a tag to another member.")]
        public async Task TransferAsync(CommandContext context, [Description("New owner of this tag")]DiscordMember newowner, 
            [Description("Name of tag to transfer"), RemainingText]string name)
        {
            if(!tryGetTag(name, context.Channel, out DatabaseTag tag))
            {
                await context.SafeRespondAsync($"⚠️ No such tag exists!");
            }

            if(!canManageTag(tag, context.Channel, context.Member))
            {
                await context.SafeRespondUnformattedAsync("⚠️ You don't own that tag!");
            }

            using (var db = this.Database.CreateContext())
            {
                tag.OwnerId = (long)newowner.Id;
                db.Tags.Update(tag);
                await db.SaveChangesAsync();
                await context.SafeRespondAsync($"✅ {(tag.ChannelId == 0 ? "Server" : "Channel")} tag `{name.BreakMentions()}` successfully transferred to {newowner.Mention}!");
            }
        }

        private bool tryGetTag(string name, DiscordChannel channel, out DatabaseTag tag)
        {
            using (var db = this.Database.CreateContext())
            {
                tag = db.Tags.FirstOrDefault(x => x.Name == name && x.ChannelId == (long)channel.Id);
                if (tag == null)
                    tag = db.Tags.FirstOrDefault(x => x.Name == name && x.GuildId == (long)channel.GuildId && x.ChannelId < 1);

                return tag != null;
            }
        }

        private bool canManageTag(DatabaseTag tag, DiscordChannel channel, DiscordMember member)
        {
            return tag.OwnerId == (long)member.Id
                || channel.Guild.OwnerId == member.Id
                || member.PermissionsIn(channel).HasPermission(Permissions.ManageMessages);
        }
    }
}
