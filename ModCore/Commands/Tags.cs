using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Utils.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [SlashCommandGroup("tag", "Commands for tags.")]
    [GuildOnly]
    public class Tags : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }
        public InteractivityExtension Interactivity { private get; set; }

        [SlashCommand("get", "Gets a tag in this channel.")]
        public async Task GetAsync(InteractionContext ctx,
            [Option("name", "Tag name.")]string name)
        {
            if (tryGetTag(name, ctx.Channel, out DatabaseTag tag))
            {
                await ctx.CreateResponseAsync($"🏷 `{name}`:\n\n{tag.Contents}");
                return;
            }

            await ctx.CreateResponseAsync($"⚠️ No such tag exists!", true);
        }

        [SlashCommand("set", "Sets a tag's content.")]
        public async Task SetAsync(InteractionContext ctx, 
            [Option("name", "Name of tag to set.")]string name, 
            [Option("content", "New content for this tag.")]string content)
        {
            bool isNew = false;
            DatabaseTag tag;

            if (tryGetTag(name, ctx.Channel, out tag))
            {
                if (!canManageTag(tag, ctx.Channel, ctx.Member))
                {
                    await ctx.CreateResponseAsync("⚠️ That tag already exists and you don't own it!", true);
                    return;
                }
            }
            else
            {
                tag = new DatabaseTag
                {
                    ChannelId = -1, // guild tag
                    Name = name,
                    GuildId = (long)ctx.Guild.Id,
                    OwnerId = (long)ctx.User.Id,
                    CreatedAt = DateTime.Now
                };
                isNew = true;
            }

            using (var db = this.Database.CreateContext())
            {
                tag.Contents = content;
                if (isNew)
                {
                    db.Tags.Add(tag);
                    await ctx.CreateResponseAsync($"✅ Tag `{name}` succesfully created!", true);
                }
                else
                {
                    db.Tags.Update(tag);
                    await ctx.CreateResponseAsync($"✅ Tag `{name}` succesfully modified!", true);
                }
                await db.SaveChangesAsync();
            }
        }

        [SlashCommand("override", "Creates a channel-specific override for a tag.")]
        public async Task OverrideAsync(InteractionContext ctx,
            [Option("name", "Name of tag to override.")] string name,
            [Option("content", "New override content for this tag.")] string content)
        {
            DatabaseTag tag;
            bool isNew = false;
            if (!tryGetTag(name, ctx.Channel, out tag)
                || tag.ChannelId < 1)
            {
                tag = new DatabaseTag()
                {
                    ChannelId = (long)ctx.Channel.Id,
                    GuildId = (long)ctx.Guild.Id,
                    Contents = content,
                    CreatedAt = DateTime.Now,
                    Name = name,
                    OwnerId = (long)ctx.User.Id
                };
                isNew = true;
            }
            else if (!canManageTag(tag, ctx.Channel, ctx.Member))
            {
                await ctx.CreateResponseAsync("⚠️ That tag already exists and you don't own it!", true);
                return;
            }

            using (var db = this.Database.CreateContext())
            {
                tag.Contents = content;
                if (isNew)
                {
                    db.Tags.Add(tag);
                    await ctx.CreateResponseAsync($"✅ Channel override tag `{name}` succesfully created!", true);
                }
                else
                {
                    db.Tags.Update(tag);
                    await ctx.CreateResponseAsync($"✅ Channel override tag `{name}` succesfully modified!", true);
                }
                await db.SaveChangesAsync();
            }
        }

        [SlashCommand("remove", "Removes a tag.")]
        public async Task RemoveAsync(InteractionContext ctx, [Option("name", "Name of the tag to remove.")]string name)
        {
            if (!tryGetTag(name, ctx.Channel, out DatabaseTag tag))
            {
                await ctx.CreateResponseAsync($"⚠️ No such tag exists!", true);
                return;
            }

            if (!canManageTag(tag, ctx.Channel, ctx.Member))
            {
                await ctx.CreateResponseAsync("⚠️ You don't own that tag!", true);
                return;
            }

            var confirmMsg = new DiscordFollowupMessageBuilder()
                .WithContent($"❗ Do you really want to delete " +
                    $"{(tag.ChannelId == 0 ? "server" : "channel")} tag `{name}`?")
                .AsEphemeral();

            var response = await ctx.ConfirmAsync(confirmMsg, "Yes, delete this tag", "Never mind.", DiscordEmoji.FromUnicode("💣"));

            if (!response.TimedOut && response.Accepted)
            {
                using (var db = this.Database.CreateContext())
                {
                    db.Tags.Remove(tag);
                    await db.SaveChangesAsync();
                    await ctx.CreateResponseAsync($"✅ Tag `{name}` successfully removed!", true);
                }
                return;
            }

            await ctx.CreateResponseAsync($"⚠️ Canceled deleting tag `{name}`", true);
        }

        [SlashCommand("info", "Shows information about a tag.")]
        public async Task InfoAsync(InteractionContext ctx, 
            [Option("name", "Name of tag to show information about.")]string name)
        {
            if (!tryGetTag(name, ctx.Channel, out DatabaseTag tag))
            {
                await ctx.CreateResponseAsync($"⚠️ No such tag exists!", true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"🏷 {name}")
                .WithDescription($"Created at: {tag.CreatedAt.ToString()}\nOwned by: <@{tag.OwnerId.ToString()}>")
                .AddField("Tag Type", tag.ChannelId == 0 ? "This tag is a server-global tag." : "This tag is a channel-specific override.")
                .AddField("Content", tag.Contents);

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("transfer", "Transfers a tag to a different user.")]
        public async Task TransferAsync(InteractionContext ctx, 
            [Option("name", "Name of tag you want to transfer.")]string name, 
            [Option("user", "User to transfer this tag to.")]DiscordUser newowner)
        {
            if (!tryGetTag(name, ctx.Channel, out DatabaseTag tag))
            {
                await ctx.CreateResponseAsync($"⚠️ No such tag exists!", true);
            }

            if (!canManageTag(tag, ctx.Channel, ctx.Member))
            {
                await ctx.CreateResponseAsync("⚠️ You don't own that tag, and you don't have `Manage Messages` permissions!", true);
            }

            using (var db = this.Database.CreateContext())
            {
                tag.OwnerId = (long)newowner.Id;
                db.Tags.Update(tag);
                await db.SaveChangesAsync();
                await ctx.CreateResponseAsync($"✅ {(tag.ChannelId == 0 ? "Server" : "Channel")} tag `{name}` successfully transferred to {newowner.Mention}!", false);
            }
        }

        [SlashCommand("list", "Lists all tags available in this channel.")]
        public async Task ListAsync(InteractionContext ctx)
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
                    await ctx.CreateResponseAsync("⚠️ There are no tags available in this channel!", true);
                }
                else
                {
                    string tags = string.Join("\n", list.Select(x => x.ChannelId < 1 ? $"🌍 `{x.Name}`" : $"💬 `{x.Name}`"));
                    var embedBase = new DiscordEmbedBuilder()
                        .WithTitle("🏷 Tags available in this channel");

                    var pages = this.Interactivity.GeneratePagesInEmbed(tags, SplitType.Line, embedBase);
                    await this.Interactivity.SendPaginatedResponseAsync(ctx.Interaction, true, ctx.User, pages, deletion: ButtonPaginationBehavior.DeleteButtons);
                }
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
