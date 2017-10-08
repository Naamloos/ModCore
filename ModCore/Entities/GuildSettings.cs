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
        public string Prefix { get; set; } = null;

        /// <summary>
        /// Gets or sets the ID of the role used to mute users from chat. Setting this to 0 will disable muting.
        /// </summary>
        [JsonProperty("mute_role_id")]
        public ulong MuteRoleId { get; set; } = 0;

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
        public bool Enable { get; set; } = false;

        /// <summary>
        /// Gets or sets the ActionLog's webhook ID.
        /// </summary>
        [JsonProperty("webhook_id")]
        public ulong WebhookId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ActionLog's webhook token.
        /// </summary>
        [JsonProperty("webhook_token")]
        public string WebhookToken { get; set; } = "";
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
        public bool BlockUrlShorteners = false;
        
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
        public bool Enable { get; set; } = false;

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
        public bool Enable { get; set; } = false;

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
}
