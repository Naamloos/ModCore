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

		[GroupCommand]
		public async Task ExecuteGroupAsync(CommandContext context, 
            [RemainingText, Description("Tag to get information about. Prefix with a channel to get it from a different channel.")] string arguments)
        {
            List<string> s = (arguments.Split(' ')).ToList();
            try
            {
                if (!s[0].StartsWith("<#"))
                    throw new Exception();
                DiscordChannel c = (DiscordChannel)await context.CommandsNext.ConvertArgument<DiscordChannel>(s[0], context);
                s.RemoveAt(0);
                await ReturnTag(context, c, string.Join(' ', s));
            }
            catch (Exception)
            {
                await ReturnTag(context, context.Channel, string.Join(' ', s));
            }
        }

        public async Task ReturnTag(CommandContext context, DiscordChannel channel, string name)
        {
            using (var db = this.Database.CreateContext())
            {
                try
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)channel.Id);
                    await context.SafeRespondAsync($"`{tag.Name.BreakMentions()}`:\n{tag.Contents.BreakMentions()}");
                }
                catch (Exception)
                {
                    await context.SafeRespondUnformattedAsync("No such tag exists!");
                }
            }
        }

        [Command("set"), CheckDisable]
        [Description("Sets a new tag for this channel, or modifies one if you own it.")]
        public async Task SetAsync(CommandContext context, [Description("Tag to create")]string name, [Description("Contents of tag"), RemainingText]string contents)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)context.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)context.Channel.Id);
                    if (tag.OwnerId != (long)context.Member.Id)
                        await context.SafeRespondUnformattedAsync("That tag already exists for this channel and you don't own it!");
                    else
                    {
                        tag.Contents = contents;
                        db.Tags.Update(tag);
                        await db.SaveChangesAsync();
                        await context.SafeRespondAsync($"Succesfully modified your tag `{name.BreakMentions()}`!");
                    }
                    return;
                }

                var newtag = new DatabaseTag
                {
                    ChannelId = (long)context.Channel.Id,
                    Contents = contents,
                    Name = name,
                    OwnerId = (long)context.Member.Id,
                    CreatedAt = DateTime.Now
                };
                db.Tags.Add(newtag);
                await db.SaveChangesAsync();
                await context.SafeRespondAsync($"Tag `{name.BreakMentions()}` succesfully created!");
            }
        }

        [Command("remove"), CheckDisable]
        [Description("Removes a tag for this channel, if it exists and you own it")]
        public async Task RemoveAsync(CommandContext context, [Description("Tag to remove"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)context.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)context.Channel.Id);

                    if ((context.Member.PermissionsIn(context.Channel) & Permissions.ManageMessages) == 0 
                        || tag.OwnerId == (long)context.Member.Id || context.Guild.Owner.Id == context.Member.Id)
                    {
                        db.Tags.Remove(tag);
                        await db.SaveChangesAsync();
                        await context.SafeRespondAsync($"Tag `{name.BreakMentions()}` successfully removed!");
                    }
                    else
                    {
                        await context.SafeRespondUnformattedAsync("You don't own that tag!");
                    }
                }
                else
                {
                    await context.SafeRespondAsync($"No such tag exists!");
                }
            }
        }

        [Command("copy"), CheckDisable]
        [Description("Copies a tag from a channel to this channel.")]
        public async Task RemoveAsync(CommandContext context, [Description("Channel the tag originated from")]DiscordChannel origin, 
            [Description("Name of tag to copy"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)origin.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)origin.Id);
                    if (!db.Tags.Any(x => x.Name == name && x.ChannelId == (long)context.Channel.Id))
                    {
                        var newtag = new DatabaseTag
                        {
                            ChannelId = (long)context.Channel.Id,
                            Contents = tag.Contents,
                            Name = tag.Name,
                            OwnerId = (long)context.Member.Id,
                            CreatedAt = DateTime.Now
                        };
                        db.Tags.Add(newtag);
                        await db.SaveChangesAsync();
                        await context.SafeRespondAsync($"Tag `{name.BreakMentions()}` successfully copied from {origin.Mention}!");
                    }
                    else
                    {
                        await context.SafeRespondAsync($"Tag `{name.BreakMentions()}` already exists in this channel!");
                    }
                }
                else
                {
                    await context.SafeRespondAsync($"No such tag exists!");
                }
            }
        }

        [Command("info"), CheckDisable]
        [Description("Shows info about a tag.")]
        public async Task InfoAsync(CommandContext context, [Description("Tag to show information about"), RemainingText] string arguments)
        {
            List<string> splitstring = (arguments.Split(' ')).ToList();
            try
            {
                if (!splitstring[0].StartsWith("<#"))
                    throw new Exception();
                DiscordChannel channel = (DiscordChannel)await context.CommandsNext.ConvertArgument<DiscordChannel>(splitstring[0], context);
                splitstring.RemoveAt(0);
                await ReturnTagInfo(context, channel, string.Join(' ', splitstring));
            }
            catch (Exception)
            {
                await ReturnTagInfo(context, context.Channel, string.Join(' ', splitstring));
            }
        }

        public async Task ReturnTagInfo(CommandContext context, DiscordChannel channel, [RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                try
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)channel.Id);
                    string owner = tag.OwnerId.ToString();
                    try
                    {
                        var tagowner = await channel.Guild.GetMemberAsync((ulong)tag.OwnerId);
                        owner = $"{tagowner.Username}#{tagowner.Discriminator} / {tagowner.Mention}";
                    }
                    catch (Exception) { }
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle(name)
                        .WithDescription($"Created at: {tag.CreatedAt.ToString()}\nOwned by: {owner}\nChannel: {channel.Mention}")
                        .AddField("Content", tag.Contents);

                    await context.ElevatedRespondAsync(embed: embed);
                }
                catch (Exception)
                {
                    await context.SafeRespondUnformattedAsync("No such tag exists!");
                }
            }
        }

        [Command("list"), CheckDisable]
        [Description("Lists tags for this channel.")]
        public async Task ListAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            using (var db = this.Database.CreateContext())
            {
				long channelid = (long)ctx.Channel.Id;
				if (channel != null)
					channelid = (long)channel.Id;

				var list = db.Tags.Where(x => x.ChannelId == channelid);
				if (list.Count() < 1)
                {
                    await ctx.SafeRespondUnformattedAsync("This channel has no tags!");
                }
                else
                {
                    string tags = string.Join("\n", list.Select(x => x.Name));
                    var pages = this.Interactivity.GeneratePagesInEmbed(tags, SplitType.Line);
                    await this.Interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages, new PaginationEmojis());
                }
            }
        }

        [Command("transfer"), CheckDisable]
        [Description("Transfers ownership of a tag to another member.")]
        public async Task TransferAsync(CommandContext context, [Description("New owner of this tag")]DiscordMember newowner, 
            [Description("Name of tag to transfer"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)context.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)context.Channel.Id);
                    if (tag.OwnerId != (long)context.Member.Id)
                        await context.SafeRespondUnformattedAsync("⚠️ You don't own that tag!");
                    else
                    {
                        tag.OwnerId = (long)newowner.Id;
                        db.Tags.Update(tag);
                        await db.SaveChangesAsync();
                        await context.SafeRespondAsync($"✅ Tag `{name.BreakMentions()}` successfully transferred to {newowner.Mention}!");
                    }
                }
                else
                {
                    await context.SafeRespondAsync($"⚠️ No such tag exists!");
                }
            }
        }
    }
}
