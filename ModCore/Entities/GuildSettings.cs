using System.Collections.Generic;
using Newtonsoft.Json;

namespace ModCore.Entities
{
    /// <summary>
    /// Represents a guild's configuration options.
    /// </summary>
    public class GuildSettings
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
        /// Gets the configuration of Invite Blocker. Invite Blocker will check each incoming message for any invites, 
        /// delete unwanted ads, and automatically ban users who spam invites.
        /// </summary>
        [JsonProperty("invite_blocker")]
        public GuildInviteBlockerSettings InviteBlocker { get; private set; } = new GuildInviteBlockerSettings();

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
    }

    /// <summary>
    /// Represents configuration for Invite Blocker.
    /// </summary>
    public class GuildInviteBlockerSettings
    {
        /// <summary>
        /// Gets or sets whether Invite Blocker should be enabled.
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
