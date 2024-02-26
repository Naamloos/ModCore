using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Commands;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("override_tag")]
    public class OverrideTagModal : IModal
    {
        [ModalField("Tag Content (Markdown Permitted)", "content", "https://www.youtube.com/watch?v=dQw4w9WgXcQ", null, true, TextInputStyle.Paragraph, 1, 255)]
        public string Content { get; set; }

        [ModalHiddenField("n")]
        public string TagName { get; set; }

        private DatabaseContextBuilder Database;

        public OverrideTagModal(DatabaseContextBuilder database)
        {
            this.Database = database;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            await interaction.DeferAsync(true);
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);

            bool isNew = false;
            if (!Tags.tryGetTag(TagName, interaction.Channel, this.Database, out var tag)
                || tag.ChannelId < 1)
            {
                tag = new DatabaseTag()
                {
                    ChannelId = (long)interaction.Channel.Id,
                    GuildId = (long)interaction.Guild.Id,
                    Contents = Content,
                    CreatedAt = DateTime.Now,
                    Name = TagName,
                    OwnerId = (long)interaction.User.Id
                };
                isNew = true;
            }
            else if (!Tags.canManageTag(tag, interaction.Channel, member))
            {
                await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"⚠️ The tag {TagName} already exists and you can not manage it!"));
                return;
            }

            await using var db = this.Database.CreateContext();
            tag.Contents = Content;
            if (isNew)
            {
                db.Tags.Add(tag);
                await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"✅ Channel override tag `{TagName}` succesfully created"));
            }
            else
            {
                db.Tags.Update(tag);
                await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"✅ Channel override tag `{TagName}` succesfully modified!"));
            }
            await db.SaveChangesAsync();
        }
    }
}
