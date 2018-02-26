using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Commands
{
    [Group("tag")]
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
		public async Task ExecuteGroupAsync(CommandContext ctx, 
            [RemainingText, Description("Tag to get information about. Prefix with a channel to get it from a different channel.")] string args)
        {
            List<string> s = (args.Split(' ')).ToList();
            try
            {
                if (!s[0].StartsWith("<#"))
                    throw new Exception();
                DiscordChannel c = (DiscordChannel)await ctx.CommandsNext.ConvertArgument<DiscordChannel>(s[0], ctx);
                s.RemoveAt(0);
                await ReturnTag(ctx, c, string.Join(' ', s));
            }
            catch (Exception)
            {
                await ReturnTag(ctx, ctx.Channel, string.Join(' ', s));
            }
        }

        public async Task ReturnTag(CommandContext ctx, DiscordChannel channel, string name)
        {
            using (var db = this.Database.CreateContext())
            {
                try
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)channel.Id);
                    await ctx.SafeRespondAsync($"`{tag.Name.BreakMentions()}`:\n{tag.Contents.BreakMentions()}");
                }
                catch (Exception)
                {
                    await ctx.SafeRespondAsync("No such tag exists!");
                }
            }
        }

        [Command("set")]
        [Description("Sets a new tag for this channel, or modifies one if you own it.")]
        public async Task SetAsync(CommandContext ctx, [Description("Tag to create")]string name, [Description("Contents of tag"), RemainingText]string contents)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id);
                    if (tag.OwnerId != (long)ctx.Member.Id)
                        await ctx.SafeRespondAsync("That tag already exists for this channel and you don't own it!");
                    else
                    {
                        tag.Contents = contents;
                        db.Tags.Update(tag);
                        await db.SaveChangesAsync();
                        await ctx.SafeRespondAsync($"Succesfully modified your tag `{name.BreakMentions()}`!");
                    }
                    return;
                }

                var t = new DatabaseTag
                {
                    ChannelId = (long)ctx.Channel.Id,
                    Contents = contents,
                    Name = name,
                    OwnerId = (long)ctx.Member.Id,
                    CreatedAt = DateTime.Now
                };
                db.Tags.Add(t);
                await db.SaveChangesAsync();
                await ctx.SafeRespondAsync($"Tag `{name.BreakMentions()}` succesfully created!");
            }
        }

        [Command("remove")]
        [Description("Removes a tag for this channel, if it exists and you own it")]
        public async Task RemoveAsync(CommandContext ctx, [Description("Tag to remove"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id);

                    if ((ctx.Member.PermissionsIn(ctx.Channel) & Permissions.ManageMessages) == 0 || tag.OwnerId == (long)ctx.Member.Id || ctx.Guild.Owner.Id == ctx.Member.Id)
                    {
                        db.Tags.Remove(tag);
                        await db.SaveChangesAsync();
                        await ctx.SafeRespondAsync($"Tag `{name.BreakMentions()}` successfully removed!");
                    }
                    else
                    {
                        await ctx.SafeRespondAsync("You don't own that tag!");
                    }
                }
                else
                {
                    await ctx.SafeRespondAsync($"No such tag exists!");
                }
            }
        }

        [Command("copy")]
        [Description("Copies a tag from a channel to this channel.")]
        public async Task RemoveAsync(CommandContext ctx, [Description("Channel the tag originated from")]DiscordChannel origin, 
            [Description("Name of tag to copy"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)origin.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)origin.Id);
                    if (!db.Tags.Any(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id))
                    {
                        var newtag = new DatabaseTag
                        {
                            ChannelId = (long)ctx.Channel.Id,
                            Contents = tag.Contents,
                            Name = tag.Name,
                            OwnerId = (long)ctx.Member.Id,
                            CreatedAt = DateTime.Now
                        };
                        db.Tags.Add(newtag);
                        await db.SaveChangesAsync();
                        await ctx.SafeRespondAsync($"Tag `{name.BreakMentions()}` successfully copied from {origin.Mention}!");
                    }
                    else
                    {
                        await ctx.SafeRespondAsync($"Tag `{name.BreakMentions()}` already exists in this channel!");
                    }
                }
                else
                {
                    await ctx.SafeRespondAsync($"No such tag exists!");
                }
            }
        }

        [Command("info")]
        [Description("Shows info about a tag.")]
        public async Task InfoAsync(CommandContext ctx, [Description("Tag to show information about"), RemainingText] string args)
        {
            List<string> s = (args.Split(' ')).ToList();
            try
            {
                if (!s[0].StartsWith("<#"))
                    throw new Exception();
                DiscordChannel c = (DiscordChannel)await ctx.CommandsNext.ConvertArgument<DiscordChannel>(s[0], ctx);
                s.RemoveAt(0);
                await ReturnTagInfo(ctx, c, string.Join(' ', s));
            }
            catch (Exception)
            {
                await ReturnTagInfo(ctx, ctx.Channel, string.Join(' ', s));
            }
        }

        public async Task ReturnTagInfo(CommandContext ctx, DiscordChannel channel, [RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                try
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)channel.Id);
                    string owner = tag.OwnerId.ToString();
                    try
                    {
                        var o = await channel.Guild.GetMemberAsync((ulong)tag.OwnerId);
                        owner = $"{o.Username}#{o.Discriminator} / {o.Mention}";
                    }
                    catch (Exception) { }
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle(name)
                        .WithDescription($"Created at: {tag.CreatedAt.ToString()}\nOwned by: {owner}\nChannel: {channel.Mention}")
                        .AddField("Content", tag.Contents);

                    await ctx.ElevatedRespondAsync(embed: embed);
                }
                catch (Exception)
                {
                    await ctx.SafeRespondAsync("No such tag exists!");
                }
            }
        }

        [Command("list")]
        [Description("Lists tags for this channel.")]
        public async Task ListAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            using (var db = this.Database.CreateContext())
            {
				long chan = (long)ctx.Channel.Id;
				if (channel != null)
					chan = (long)channel.Id;

				var list = db.Tags.Where(x => x.ChannelId == chan);
				if (list.Count() < 1)
                {
                    await ctx.SafeRespondAsync("This channel has no tags!");
                }
                else
                {
                    string tags = string.Join("\n", list.Select(x => x.Name));
                    var p = this.Interactivity.GeneratePagesInEmbeds(tags);
                    await this.Interactivity.SendPaginatedMessage(ctx.Channel, ctx.Member, p);
                }
            }
        }

        [Command("transfer")]
        [Description("Transfers ownership of a tag to another member.")]
        public async Task TransferAsync(CommandContext ctx, [Description("New owner of this tag")]DiscordMember newowner, 
            [Description("Name of tag to transfer"), RemainingText]string name)
        {
            using (var db = this.Database.CreateContext())
            {
                if (db.Tags.Any(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id))
                {
                    var tag = db.Tags.First(x => x.Name == name && x.ChannelId == (long)ctx.Channel.Id);
                    if (tag.OwnerId != (long)ctx.Member.Id)
                        await ctx.SafeRespondAsync("You don't own that tag!");
                    else
                    {
                        tag.OwnerId = (long)newowner.Id;
                        db.Tags.Update(tag);
                        await db.SaveChangesAsync();
                        await ctx.SafeRespondAsync($"Tag `{name.BreakMentions()}` successfully transferred to {newowner.Mention}!");
                    }
                }
                else
                {
                    await ctx.SafeRespondAsync($"No such tag exists!");
                }
            }
        }
    }
}
