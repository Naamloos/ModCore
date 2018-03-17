using System.Collections.Generic;
using Newtonsoft.Json;

namespace ModCore.Entities
{
    /// <summary>
    /// Represents a guild's configuration options.
    /// </summary>
    public partial class GuildSettings
    {
        /// <summary>
        /// Gets or sets the command prefix for the guild.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the ID of the role used to mute users from chat. Setting this to 0 will disable muting.
        /// </summary>
        [JsonProperty("mute_role_id")]
        public ulong MuteRoleId { get; set; }

        /// <summary>
        /// Gets the configuration of Linkfilter™. The Linkfilter™ is the all-new system for Filtering Bad Links out of,
        /// your guild. It'll do everything, from trashing invite links to ruthlessly annihilating IP loggers, URL shorteners
        /// and other suspicious sites.
        /// </summary>
        [JsonProperty("linkfilter")]
        public GuildLinkfilterSettings Linkfilter { get; private set; } = new GuildLinkfilterSettings();

        /// <summary>
        /// Gets the configuration for InvisiCop. InvisiCop removes messages from users set to invisible, since they 
        /// break user caches.
        /// </summary>
        [JsonProperty("invisicop")]
        public GuildInvisiCopSettings InvisiCop { get; private set; } = new GuildInvisiCopSettings();

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
        public GuildActionLogSettings ActionLog { get; private set; } = new GuildActionLogSettings();

        /// <summary>
        /// Gets the configuration for AutoRole. AutoRole automatically grants a role to a member on join if there's no existing rolestate.
        /// </summary>
        [JsonProperty("autorole")]
        public GuildAutoRoleSettings AutoRole { get; private set; } = new GuildAutoRoleSettings();

        /// <summary>
        /// Gets the configuration for CommandError. CommandErrors logs command errors to chat or action log.
        /// </summary>
        [JsonProperty("commanderror")]
        public GuildCommandErrorSettings CommandError { get; private set; } = new GuildCommandErrorSettings();

        /// <summary>
        /// Gets the configuration for JoinLog. JoinLog logs new memebrs to a channel.
        /// </summary>
        [JsonProperty("joinlog")]
        public GuildJoinLogSettings JoinLog { get; private set; } = new GuildJoinLogSettings();

        /// <summary>
        /// Gets the SelfRoles for this guild. SelfRoles are roles members can grant themselves.
        /// </summary>
        [JsonProperty("selfroles")]
        public List<ulong> SelfRoles { get; private set; } = new List<ulong>();

        /// <summary>
        /// Gets the Starboard settings for this guild. Starboard lets users star messages they like.
        /// </summary>
        [JsonProperty]
        public GuildStarboardSettings Starboard { get; private set; } = new GuildStarboardSettings();

        /// <summary>
        /// Gets the GlobalWarn settings for this guild. GlobalWarn notifies the server owner when a previously 
        /// banned user joins, or alternatively bans said user.
        /// </summary>
        [JsonProperty]
        public GuildGlobalWarnSettings GlobalWarn { get; private set; } = new GuildGlobalWarnSettings();

        /// <summary>
        /// Gets whether spelling helper is enabled or disabled for this guild.
        /// </summary>
        [JsonProperty("spellhelp")]
        public bool SpellingHelperEnabled { get; set; }
    }

    public class GuildStarboardSettings
    {
        /// <summary>
        /// Gets or sets the Starboard channel ID.
        /// </summary>
        [JsonProperty("channel_id")]
        public long ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the starboard emoji.
        /// </summary>
        [JsonProperty("emoji")]
        public GuildStarboardEmoji Emoji { get; set; } = new GuildStarboardEmoji();

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

    public class GuildStarboardEmoji
    {
        [JsonProperty("id")]
        public long EmojiId { get; set; }

        [JsonProperty("name")]
        public string EmojiName { get; set; } = "⭐";
    }

    /// <summary>
    /// Represents configuration for JoinLog
    /// </summary>
    public class GuildJoinLogSettings
    {
        /// <summary>
        /// Gets or sets whether JoinLog should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the JoinLog channel ID.
        /// </summary>
        [JsonProperty("channel_id")]
        public long ChannelId { get; set; }
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
        [JsonProperty("role_id")]
        public long RoleId { get; set; }
    }

    /// <summary>
    /// Represents configuration for the ActionLog.
    /// </summary>
    public class GuildActionLogSettings
    {
        /// <summary>
        /// Gets or sets whether the ActionLog should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets the ActionLog's webhook ID.
        /// </summary>
        [JsonProperty("webhook_id")]
        public ulong WebhookId { get; set; }

        /// <summary>
        /// Gets or sets the ActionLog's webhook token.
        /// </summary>
        [JsonProperty("webhook_token")]
        public string WebhookToken { get; set; } = "";
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
        public List<ulong> ExemptRoleIds { get; private set; } = new List<ulong>();

        /// <summary>
        /// Gets the list of users who are exempt from Invite Blocker.
        /// </summary>
        [JsonProperty("exempt_user_ids")]
        public List<ulong> ExemptUserIds { get; private set; } = new List<ulong>();

        /// <summary>
        /// Gets the list of guilds which are exempt from being flagged for ads.
        /// </summary>
        [JsonProperty("exempt_invite_guild_ids")]
        public List<ulong> ExemptInviteGuildIds { get; private set; } = new List<ulong>();
        
        /// <summary>
        /// Gets the list of guilds which are exempt from being flagged for ads.
        /// </summary>
        [JsonProperty("custom_link_filters")]
        public List<string> CustomLinkFilters { get; private set; } = new List<string>();
        
        /// <summary>
        /// Toggles blocking invite links, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_invite_links")]
        public bool BlockInviteLinks = true;
        
        /// <summary>
        /// Toggles blocking IP logging sites, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_ip_loggers")]
        public bool BlockIpLoggers = true;
        
        /// <summary>
        /// Toggles blocking DDoS sites, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_booters")]
        public bool BlockBooters = true;
        
        /// <summary>
        /// Toggles blocking URL shorteners, unless posted by a member with 'Manage Messages' permission or equivalent.
        /// </summary>
        [JsonProperty("block_url_shorteners")]
        public bool BlockUrlShorteners;
        
        /// <summary>
        /// Toggles blocking shock sites, screamers and gore sites, unless posted by a member with 'Manage Messages'
        /// permission or equivalent.
        /// </summary>
        [JsonProperty("block_shock_sites")]
        public bool BlockShockSites = true;
    }

    /// <summary>
    /// Represents configuration for InvisiCop.
    /// </summary>
    public class GuildInvisiCopSettings
    {
        /// <summary>
        /// Gets or sets whether InvisiCop should be enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enable { get; set; }

        /// <summary>
        /// Gets the list of roles which are exempt from InvisiCop checks.
        /// </summary>
        [JsonProperty("exempt_role_ids")]
        public List<ulong> ExemptRoleIds { get; private set; } = new List<ulong>();

        /// <summary>
        /// Gets the list of users who are exempt from InvisiCop checks.
        /// </summary>
        [JsonProperty("exempt_user_ids")]
        public List<ulong> ExemptUserIds { get; private set; } = new List<ulong>();
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
        public List<ulong> IgnoredRoleIds { get; private set; } = new List<ulong>();

        /// <summary>
        /// Gets the list of channels which are ignored by Role State. Overwrites for these channels won't be saved or 
        /// restored.
        /// </summary>
        [JsonProperty("ignored_channel_ids")]
        public List<ulong> IgnoredChannelIds { get; private set; } = new List<ulong>();
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
}
