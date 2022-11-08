using System.Collections.Generic;
using System.Threading.Tasks;
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

		[JsonProperty("welcome")]
		public WelcomeSettings Welcome { get; private set; } = new WelcomeSettings();

        [JsonProperty("nicknameconfirm")]
        public NicknameConfirmSettings NicknameConfirm { get; set; } = new NicknameConfirmSettings();

        [JsonProperty("levels")]
        public LevelSettings Levels = new LevelSettings();
    }

    public class NicknameConfirmSettings
    {
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }

    public class LevelSettings
    {
        [JsonProperty("levels_enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("messages_enabled")]
        public bool MessagesEnabled { get; set; }

        [JsonProperty("redirect_messages")]
        public bool RedirectMessages { get; set; }

        [JsonProperty("message_channel_id")]
        public ulong ChannelId { get; set; }
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
        public ulong ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the starboard emoji.
        /// </summary>
        [JsonProperty("emoji")]
        public GuildEmoji Emoji { get; set; } = new GuildEmoji();

	    /// <summary>
	    /// Gets or sets whether starboard should be enabled.
	    /// </summary>
	    [JsonProperty("enabled")]
        public bool Enable { get; set; }

        [JsonProperty("minimum")]
        public int Minimum { get; set; } = 3;

	    [JsonProperty("allow_nsfw")] 
	    public bool AllowNSFW { get; set; }
    }

    public class GuildEmoji
    {
        [JsonProperty("id")]
        public ulong EmojiId { get; set; }

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
	    public bool Enable { get; set; }

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
	    public bool JoinLog_Enable { get; set; }

        [JsonProperty("editlog_enabled")]
        public bool EditLog_Enable { get; set; }

        [JsonProperty("nicknamelog_enabled")]
        public bool NickameLog_Enable { get; set; }

        [JsonProperty("invitelog_enabled")]
        public bool InviteLog_Enable { get; set; }

        [JsonProperty("avatarlog_enabled")]
        public bool AvatarLog_Enable { get; set; }

        [JsonProperty("modlog_enabled")]
        public bool ModLog_Enable { get; set; }

        /// <summary>
        /// Gets or sets logging channel ID
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }

    public class GuildCommandErrorSettings
    {
        /// <summary>
        /// Gets or sets the command error verbosity for chat
        /// </summary>
        [JsonProperty("chatverbosity")]
        public CommandErrorVerbosity Chat { get; set; } = CommandErrorVerbosity.None;

	    /// <summary>
	    /// Gets or sets the command error verbosity for the action log (if enabled)
	    /// </summary>
	    [JsonProperty("actionverbosity")]
	    public CommandErrorVerbosity ActionLog { get; set; } = CommandErrorVerbosity.None;
    }

    public enum CommandErrorVerbosity
    {
        None,
        Name,
        NameDesc,
        Exception
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
	    public bool Enable { get; set; }

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
	    public bool Enable { get; set; }

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
        public bool Nickname { get; set; }
    }

    public class GuildGlobalWarnSettings
    {
	    /// <summary>
	    /// Gets or sets whether GlobalWarn should be enabled.
	    /// </summary>
	    [JsonProperty("enabled")] 
	    public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the GlobalWarn 
        /// </summary>
        [JsonProperty("warnlevel")]
        public GlobalWarnLevel WarnLevel { get; set; } = GlobalWarnLevel.None;
    }

    public enum GlobalWarnLevel
    {
        None,
        Owner,
        JoinLog
    }

    public class GuildBotManagerSettings
    {
        /// <summary>
        /// Gets or sets whether anyone but the owner has access to scary bot commands.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the permission level  
        /// </summary>
        [JsonProperty("permissionlevel")]
        public BotManagerPermissionLevel PermissionLevel { get; set; } = BotManagerPermissionLevel.None;
    }

    public enum BotManagerPermissionLevel
    {
        None,
        Update,
        All
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
		public ulong ChannelId { get; set; }

		/// <summary>
		/// Message reaction is on.
		/// </summary>
		[JsonProperty("message_id")]
		public ulong MessageId { get; set; }

		/// <summary>
		/// Role to grant.
		/// </summary>
		[JsonProperty("role_id")]
		public ulong RoleId { get; set; }

		/// <summary>
		/// Reaction to grant role on.
		/// </summary>
		[JsonProperty("reaction")]
		public GuildEmoji Reaction { get; set; }
	}

	/// <summary>
	/// Represents settings for welcome messages.
	/// </summary>
	public class WelcomeSettings
	{
		[JsonProperty("enabled")]
		public bool Enable { get; set; }

        [JsonProperty("message")]
		public string Message { get; set; } = "";

		[JsonProperty("channel_id")]
		public ulong ChannelId { get; set; }

		[JsonProperty("is_embed")]
		public bool IsEmbed { get; set; }
	}
}
