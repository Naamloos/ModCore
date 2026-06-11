using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Utils
{
    public class MessageBuilder
    {
        private string? _content;
        private bool _tts;
        private List<Embed>? _embeds;
        private AllowedMention? _allowedMentions;
        private MessageReference? _messageReference;
        private List<Component> _components = new();
        private MessageFlags _flags;
        private bool _isComponentsV2;

        public MessageBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public MessageBuilder WithTts(bool tts = true)
        {
            _tts = tts;
            return this;
        }

        public MessageBuilder AddEmbed(Embed embed)
        {
            _embeds ??= new List<Embed>();
            _embeds.Add(embed);
            return this;
        }

        public MessageBuilder AddEmbed(Action<EmbedBuilder> configure)
        {
            var builder = new EmbedBuilder();
            configure(builder);
            return AddEmbed(builder.Build());
        }

        public MessageBuilder WithAllowedMentions(AllowedMention allowedMentions)
        {
            _allowedMentions = allowedMentions;
            return this;
        }

        public MessageBuilder WithReply(Snowflake messageId, Snowflake? guildId = null, bool failIfNotExists = true)
        {
            _messageReference = new MessageReference
            {
                MessageId = messageId,
                GuildId = guildId is not null ? guildId : Optional.None,
                FailIfNotExists = failIfNotExists
            };
            return this;
        }

        public MessageBuilder AddComponent(Component component)
        {
            _components.Add(component);
            return this;
        }

        public MessageBuilder AddActionRow(Action<ActionRowBuilder> configure)
        {
            var builder = new ActionRowBuilder();
            configure(builder);
            _components.Add(builder.Build());
            return this;
        }

        public MessageBuilder AddContainer(Action<ContainerBuilder> configure)
        {
            _isComponentsV2 = true;
            var builder = new ContainerBuilder();
            configure(builder);
            _components.Add(builder.Build());
            return this;
        }

        public MessageBuilder WithFlags(MessageFlags flags)
        {
            _flags = flags;
            return this;
        }

        public MessageBuilder SuppressEmbeds(bool suppress = true)
        {
            if (suppress)
                _flags |= MessageFlags.SuppressEmbeds;
            else
                _flags &= ~MessageFlags.SuppressEmbeds;
            return this;
        }

        public CreateMessage Build()
        {
            var finalFlags = _flags;
            if (_isComponentsV2)
            {
                // auto-enable the ComponentsV2 flag if needed
                finalFlags |= (MessageFlags)32768;
            }

            return new CreateMessage
            {
                Content = _isComponentsV2 ? Optional.None : (_content is not null ? _content : Optional.None),
                IsTTs = _tts ? _tts : Optional.None,
                Embeds = _isComponentsV2 ? [] : (_embeds?.ToArray() ?? []),
                AllowedMentions = _allowedMentions is not null? _allowedMentions : Optional.None,
                MessageReference = _messageReference is not null ? _messageReference : Optional.None,
                Components = _components?.ToArray() ?? [],
                Flags = finalFlags
            };
        }

        /// <summary>
        /// Outputs an interaction-specific envelope containing the layout data.
        /// </summary>
        public InteractionMessageResponse BuildInteractionResponse()
        {
            var messageData = Build();

            return new InteractionMessageResponse
            {
                Content = messageData.Content,
                IsTTS = messageData.IsTTs,
                Embeds = messageData.Embeds,
                AllowedMentions = messageData.AllowedMentions,
                Components = messageData.Components,
                Flags = messageData.Flags
            };
        }
    }

    public class SectionBuilder
    {
        private readonly List<Component> _sectionChildren = new();

        public SectionBuilder AddText(string markdownText)
        {
            _sectionChildren.Add(new TextDisplay { Content = markdownText });
            return this;
        }

        public Section Build()
        {
            var section = new Section();
            section.Components = _sectionChildren;
            return section;
        }
    }
    public class ActionRowBuilder
    {
        private readonly List<Component> _childComponents = new();

        public ActionRowBuilder AddButton(string customId, string label, ButtonStyle style = ButtonStyle.Primary, bool disabled = false, Emoji? emoji = null)
        {
            _childComponents.Add(new Button
            {
                CustomId = customId,
                Label = label,
                Style = style,
                Disabled = disabled,
                Emoji = emoji is not null? emoji : Optional.None
            });
            return this;
        }

        public ActionRowBuilder AddLinkButton(string url, string label, bool disabled = false, Emoji? emoji = null)
        {
            _childComponents.Add(new Button
            {
                Url = url,
                Label = label,
                Style = ButtonStyle.Link,
                Disabled = disabled,
                Emoji = emoji is not null ? emoji : Optional.None
            });
            return this;
        }

        public ActionRowBuilder AddSelectMenu(string customId, List<SelectOption> options, string? placeholder = null)
        {
            _childComponents.Add(new StringSelect
            {
                CustomId = customId,
                Options = options,
                Placeholder = placeholder is not null ? placeholder : Optional.None
            });
            return this;
        }

        public ActionRow Build() => new ActionRow { Components = _childComponents };
    }

    public class ContainerBuilder
    {
        private int? _accentColor;
        private bool? _spoiler;
        private readonly List<Component> _nestedComponents = new();

        public ContainerBuilder WithAccentColor(int hexColor)
        {
            _accentColor = hexColor;
            return this;
        }

        public ContainerBuilder IsSpoiler(bool spoiler = true)
        {
            _spoiler = spoiler;
            return this;
        }

        public ContainerBuilder AddText(string content)
        {
            _nestedComponents.Add(new TextDisplay { Content = content });
            return this;
        }

        public ContainerBuilder AddSection(Action<SectionBuilder> configure, Component accessory)
        {
            var sectionBuilder = new SectionBuilder();
            configure(sectionBuilder);

            var section = sectionBuilder.Build();
            if (accessory != null)
            {
                section.Accessory = accessory;
            }

            _nestedComponents.Add(section);
            return this;
        }

        public ContainerBuilder AddSeparator(bool visible = true, int? spacing = null)
        {
            _nestedComponents.Add(new Separator
            {
                Divider = visible,
                Spacing = spacing is not null ? (int)spacing : Optional.None
            });
            return this;
        }

        public ContainerBuilder AddActionRow(Action<ActionRowBuilder> configure)
        {
            var rowBuilder = new ActionRowBuilder();
            configure(rowBuilder);
            _nestedComponents.Add(rowBuilder.Build());
            return this;
        }

        public Container Build() => new Container
        {
            AccentColor = _accentColor is not null ? _accentColor : Optional.None,
            Spoiler = _spoiler is not null ? (bool)_spoiler : Optional.None,
            Components = _nestedComponents
        };
    }

    public class EmbedBuilder
    {
        private Embed _embed = new();
        private List<EmbedField>? _fields;

        public EmbedBuilder WithTitle(string title) { _embed = _embed with { Title = title }; return this; }
        public EmbedBuilder WithDescription(string description) { _embed = _embed with { Description = description }; return this; }
        public EmbedBuilder WithColor(int hexColor) { _embed = _embed with { Color = hexColor }; return this; }

        public EmbedBuilder AddField(string name, string value, bool inline = false)
        {
            _fields ??= new List<EmbedField>();
            _fields.Add(new EmbedField()
            {
                Name = name,
                Value = value,
                Inline = inline
            });
            return this;
        }

        public Embed Build() => _embed with { Fields = _fields is not null? _fields : Optional.None };
    }
}