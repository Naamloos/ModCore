using System.Collections.Generic;
using Newtonsoft.Json;

namespace ModCore.Database.JsonEntities
{
    /// <summary>
    /// Represents a guild's configuration options.
    /// </summary>
    public partial class GuildSettings
    {
        /// <summary>
        /// Gets the configuration of Linkfilter™. The Linkfilter™ is the all-new system for Filtering Bad Links out of,
        /// your guild. It'll do everything, from trashing invite links to ruthlessly annihilating IP loggers, URL shorteners
        /// and other suspicious sites.
        /// </summary>
        [JsonProperty("linkfilter")]
        public GuildLinkfilterSettings Linkfilter { get; private set; } = new GuildLinkfilterSettings();

        /// <summary>
        /// Gets the configuration for Role State. Role State is used to persist roles and overwrites for users who 
        /// leave the guild.
        /// </summary>
        [JsonProperty("role_state")]
        public GuildRoleStateConfig RoleState { get; private set; } = new GuildRoleStateConfig();

        /// <summary>
        /// Gets the configuration for the ActionLog. ActionLog logs actions by moderators to a webhook URL.
        /// </summary>
        [JsonProperty("actionlog")]
        public GuildLogSettings Logging { get; private set; } = new GuildLogSettings();

        /// <summary>
        /// Gets the configuration for AutoRole. AutoRole automatically grants a role to a member on join if there's no existing rolestate.
        /// </summary>
        [JsonProperty("autorole")]
        public GuildAutoRoleSettings AutoRole { get; private set; } = new GuildAutoRoleSettings();

        /// <summary>
        /// Gets the SelfRoles for this guild. SelfRoles are roles members can grant themselves.
        /// </summary>
        [JsonProperty("selfroles")]
        public HashSet<ulong> SelfRoles { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// Gets the Starboard settings for this guild. Starboard lets users star messages they like.
        /// </summary>
        [JsonProperty]
        public GuildStarboardSettings Starboard { get; private set; } = new GuildStarboardSettings();

        /// <summary>
        /// Gets the list of Reaction Roles for this guild.
        /// </summary>
        [JsonProperty("reactionroles")]
        public List<GuildReactionRole> ReactionRoles { get; private set; } = new List<GuildReactionRole>();

        /// <summary>
        /// Gets Welcomer configuration for this guild
        /// </summary>
		[JsonProperty("welcome")]
        public WelcomeSettings Welcome { get; private set; } = new WelcomeSettings();

        /// <summary>
        /// Gets nickname confirmation configuration for this guild
        /// </summary>
        [JsonProperty("nicknameconfirm")]
        public NicknameConfirmSettings NicknameConfirm { get; set; } = new NicknameConfirmSettings();

        /// <summary>
        /// Gets levels configuration for this guild
        /// </summary>
        [JsonProperty("levels")]
        public LevelSettings Levels = new LevelSettings();

        [JsonProperty("role_menus")]
        public List<GuildRoleMenu> RoleMenus = new List<GuildRoleMenu>();

        public EmbedMessageLinksMode EmbedMessageLinks = EmbedMessageLinksMode.Disabled;
    }

    public enum EmbedMessageLinksMode
    {
        Disabled = 0,
        Prefixed = 1,
        Always = 2
    }

    public class GuildRoleMenu
    {
        [JsonProperty("menu_id")]
        public string Name { get; set; }

        [JsonProperty("creator_id")]
        public ulong CreatorId { get; set; }

        [JsonProperty("role_ids")]
        public List<ulong> RoleIds { get; set; } = new List<ulong>();
    }

    public class NicknameConfirmSettings
    {
        [JsonProperty("enable")]
        public bool Enable { get; set; } = false;

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; } = 0;
    }

    public class LevelSettings
    {
        [JsonProperty("levels_enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty("messages_enabled")]
        public bool MessagesEnabled { get; set; } = false;

        [JsonProperty("redirect_messages")]
        public bool RedirectMessages { get; set; } = false;

        [JsonProperty("message_channel_id")]
        public ulong ChannelId { get; set; } = 0;
    }

    public class GuildChangeLogSettings
    {
        [JsonProperty("log_channel")]
        public long LogChannel { get; set; } = 0;

        [JsonProperty("log_nickname")]
        public bool LogNickname { get; set; } = false;

        [JsonProperty("log_avatar")]
        public bool LogAvatar { get; set; } = false;

        [JsonProperty("log_username")]
        public bool LogRoles { get; set; } = false;
    }

    public class GuildStarboardSettings
    {
        /// <summary>
        /// Gets or sets the Starboard channel ID.
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the starboard emoji.
        /// </summary>
        [JsonProperty("emoji")]
        public GuildEmoji Emoji { get; set; } = new GuildEmoji();

        /// <summary>
        /// Gets or sets whether starboard should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; } = false;

        [JsonProperty("minimum")]
        public int Minimum { get; set; } = 3;

        [JsonProperty("allow_nsfw")]
        public bool AllowNSFW { get; set; } = false;
    }

    public class GuildEmoji
    {
        [JsonProperty("id")]
        public ulong EmojiId { get; set; } = 0;

        [JsonProperty("name")]
        public string EmojiName { get; set; } = "⭐";

        public string GetStringRepresentation()
        {
            return EmojiId > 1 ? $"<:{EmojiName}:{EmojiId}>" : EmojiName;
        }
    }

    /// <summary>
    /// Represents configuration for AutoRole.
    /// </summary>
    public class GuildAutoRoleSettings
    {
        /// <summary>
        /// Gets or sets whether AutoRole should be enabled.
        /// </summary>
        [JsonProperty("enables")]
        public bool Enable { get; set; } = false;

        /// <summary>
        /// Gets or sets the AutoRole role ID.
        /// </summary>
        [JsonProperty("role_ids")]
        public List<ulong> RoleIds { get; set; } = new List<ulong>();
    }

    /// <summary>
    /// Represents configuration for the ActionLog.
    /// </summary>
    public class GuildLogSettings
    {
        /// <summary>
        /// Gets or sets whether the JoinLog should be enabled.
        /// </summary>
        [JsonProperty("joinlog_enabled")]
        public bool JoinLog_Enable { get; set; } = false;

        [JsonProperty("editlog_enabled")]
        public bool EditLog_Enable { get; set; } = false;

        [JsonProperty("nicknamelog_enabled")]
        public bool NickameLog_Enable { get; set; } = false;

        [JsonProperty("invitelog_enabled")]
        public bool InviteLog_Enable { get; set; } = false;

        [JsonProperty("avatarlog_enabled")]
        public bool AvatarLog_Enable { get; set; } = false;

        [JsonProperty("roles_enabled")]
        public bool RoleLog_Enable { get; set; } = false;

        [JsonProperty("modlog_enabled")]
        public bool ModLog_Enable { get; set; } = false;

        /// <summary>
        /// Gets or sets logging channel ID
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; } = 0;
    }

    /// <summary>
    /// Represents configuration for the Linkfilter™.
    /// </summary>
    public class GuildLinkfilterSettings
    {
        /// <summary>
        /// Gets or sets whether Linkfilter™ should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of invites after which the user gets automatically banned for ads. Set to 0 to disable automatic bans.
        /// </summary>
        [JsonProperty("ban_threshold")]
        public int BanThreshold { get; set; } = 3;

        /// <summary>
        /// Gets the list of roles which are exempt from Invite Checks.
        /// </summary>
        [JsonProperty("exempt_role_ids")]
        public HashSet<ulong> ExemptRoleIds { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// Gets the list of users who are exempt from Invite Blocker.
        /// </summary>
        [JsonProperty("exempt_user_ids")]
        public HashSet<ulong> ExemptUserIds { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// Gets the list of guilds which are exempt from being flagged for ads.
        /// </summary>
        [JsonProperty("exempt_invite_guild_ids")]
        public HashSet<ulong> ExemptInviteGuildIds { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// TODO wtf does this do
        /// </summary>
        [JsonProperty("custom_link_filters")]
        public List<string> CustomLinkFilters { get; private set; } = new List<string>();

        /// <summary>
        /// Toggles blocking invite links, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_invite_links")]
        public bool BlockInviteLinks { get; set; } = true;

        /// <summary>
        /// Toggles blocking IP logging sites, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_ip_loggers")]
        public bool BlockIpLoggers { get; set; } = true;

        /// <summary>
        /// Toggles blocking DDoS sites, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_booters")]
        public bool BlockBooters { get; set; } = true;

        /// <summary>
        /// Toggles blocking URL shorteners, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_url_shorteners")]
        public bool BlockUrlShorteners { get; set; }

        /// <summary>
        /// Toggles blocking shock sites, screamers and gore sites, unless posted by a member with 'Manage Messages'
        /// permission or equivalent.
        /// </summary>
        [JsonProperty("block_shock_sites")]
        public bool BlockShockSites { get; set; } = true;
    }

    /// <summary>
    /// Represents configuration for Role State.
    /// </summary>
    public class GuildRoleStateConfig
    {
        /// <summary>
        /// Gets or sets whether Role State should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; } = false;

        /// <summary>
        /// Gets the list of roles which are ignored by Role State. These roles won't be saved or restored.
        /// </summary>
        [JsonProperty("ignored_role_ids")]
        public HashSet<ulong> IgnoredRoleIds { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// Gets the list of channels which are ignored by Role State. Overwrites for these channels won't be saved or 
        /// restored.
        /// </summary>
        [JsonProperty("ignored_channel_ids")]
        public HashSet<ulong> IgnoredChannelIds { get; private set; } = new HashSet<ulong>();

        /// <summary>
        /// Restores nicknames on rejoin
        /// </summary>
        [JsonProperty("nickname")]
        public bool Nickname { get; set; } = false;
    }

    /// <summary>
    /// Represents a ReactionRole.
    /// </summary>
    public class GuildReactionRole
    {
        /// <summary>
        /// Channel message is in.
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; } = 0;

        /// <summary>
        /// Message reaction is on.
        /// </summary>
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; } = 0;

        /// <summary>
        /// Role to grant.
        /// </summary>
        [JsonProperty("role_id")]
        public ulong RoleId { get; set; } = 0;

        /// <summary>
        /// Reaction to grant role on.
        /// </summary>
        [JsonProperty("reaction")]
        public GuildEmoji Reaction { get; set; } = new GuildEmoji();
    }

    /// <summary>
    /// Represents settings for welcome messages.
    /// </summary>
    public class WelcomeSettings
    {
        [JsonProperty("enabled")]
        public bool Enable { get; set; } = false;

        [JsonProperty("message")]
        public string Message { get; set; } = "";

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; } = 0;

        [JsonProperty("is_embed")]
        public bool IsEmbed { get; set; } = false;
    }
}
