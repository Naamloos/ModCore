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
    [Modal("set_tag")]
    public class SetTagModal : IModal
    {
        [ModalField("Tag Content (Markdown Permitted)", "content", "https://www.youtube.com/watch?v=dQw4w9WgXcQ", null, true, TextInputStyle.Paragraph, 1, 255)]
        public string Content { get; set; }

        [ModalHiddenField("n")]
        public string TagName { get; set; }

        private DatabaseContextBuilder Database;

        public SetTagModal(DatabaseContextBuilder database)
        {
            this.Database = database;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            await interaction.DeferAsync(true);

            bool isNew = false;
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);

            if (Tags.tryGetTag(TagName, interaction.Channel, Database, out var tag))
            {
                if (!Tags.canManageTag(tag, interaction.Channel, member))
                {
                    await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"⚠️ The tag {TagName} already exists and you can not manage it!"));
                    return;
                }
            }
            else
            {
                tag = new DatabaseTag
                {
                    ChannelId = -1, // guild tag
                    Name = TagName,
                    GuildId = (long)interaction.Guild.Id,
                    OwnerId = (long)interaction.User.Id,
                    CreatedAt = DateTime.Now
                };
                isNew = true;
            }

            await using var db = this.Database.CreateContext();
            tag.Contents = Content;
            if (isNew)
            {
                db.Tags.Add(tag);
                await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"✅ Server-global tag `{TagName}` succesfully created!"));
            }
            else
            {
                db.Tags.Update(tag);
                await interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"✅ Server-global tag `{TagName}` succesfully modified!"));
            }
            await db.SaveChangesAsync();
        }
    }
}
