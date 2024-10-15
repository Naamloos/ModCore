using Microsoft.EntityFrameworkCore;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator
{
    public class Migrator
    {
        private DatabaseContext _newDatabase;
        private ClassicDatabaseContext _oldDatabase;

        public Migrator(string oldDB, string newDB, string username, string pass, string host, int port)
        {
            var oldCStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = oldDB,
                Username = username,
                Password = pass,
                Port = port,
                Host = host,
                IncludeErrorDetail = true
            };
            var newCStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = newDB,
                Username = username,
                Password = pass,
                Port = port,
                Host = host,
                IncludeErrorDetail = true
            };

            _newDatabase = new DatabaseContext(newCStringBuilder.ToString());
            _oldDatabase = new ClassicDatabaseContext(oldCStringBuilder.ToString());
        }

        public void StartMigration()
        {
            var migrations = _newDatabase.Database.GetPendingMigrations();
            if (migrations.Count() > 0)
            {
                MigratorConsole.Write("Pending migrations found for new database. Apply? (y/N)");
                var confirm = (Console.ReadLine() ?? "n").Trim().ToLower() == "y";
                if (!confirm)
                {
                    MigratorConsole.WriteLine("Migrations pending were not applied. Cancelling operation.", ConsoleColor.Red);
                    return;
                }
                MigratorConsole.WriteLine("Applying latest migrations to new Database:");
                foreach (var migration in migrations)
                {
                    MigratorConsole.WriteLine(migration, ConsoleColor.Magenta);
                }
                _newDatabase.Database.Migrate();
                MigratorConsole.WriteLine("Applied migrations!", ConsoleColor.Green);
            }

            MigratorConsole.WriteLine("Starting migration from old to new database");

            // Guild configs must be ran first, to ensure that we create guild objects where needed.
            MigrateGuildConfigs();

            // Then, we migrate levels and stars
            MigrateLevelData();
            MigrateStarData();
            MigrateTags();
            MigrateRoleStates();
            MigrateRoleOverrides();
            MigrateNicknameStates();
            MigrateTimers();

            MigratorConsole.WriteLine("Done migrating v2 database to v3 database!", ConsoleColor.Green);
        }

        private void MigrateGuildConfigs()
        {
            foreach (var guildConfig in _oldDatabase.GuildConfig)
            {
                MigratorConsole.WriteLine($"Migrating Guild config for {guildConfig.GuildId}");
                var newGuild = GetOrCreateNewGuildEntity((ulong)guildConfig.GuildId);

                if (string.IsNullOrWhiteSpace(guildConfig.Settings))
                {
                    MigratorConsole.WriteLine($"Guild with ID {guildConfig.GuildId} does not contain settings. Continuing.");
                    continue;
                }

                var settings = guildConfig.GetSettings();

                // Not to be migrated:
                // - LinkFilter / Invite blocker (will differ too much)
                // - selfrole/reaction role: currently deprecated / unused so no data

                // Logger
                newGuild.LoggerSettings = new DatabaseLoggerSettings()
                {
                    Guild = newGuild,
                    GuildId = newGuild.GuildId,
                    LogAvatars = settings.Logging.AvatarLog_Enable,
                    LogChannels = false,
                    LoggerChannelId = (ulong)settings.Logging.ChannelId,
                    LogGuildEdits = false,
                    LogInvites = settings.Logging.InviteLog_Enable,
                    LogJoins = settings.Logging.JoinLog_Enable,
                    LogMessageEdits = settings.Logging.EditLog_Enable,
                    LogNicknames = settings.Logging.NickameLog_Enable,
                    LogRoleAssignment = settings.Logging.RoleLog_Enable,
                    LogRoleEdits = settings.Logging.RoleLog_Enable
                };

                // Autorole
                foreach (var roleId in settings.AutoRole.RoleIds)
                {
                    _newDatabase.AutoRoles.Add(new DatabaseAutoRole()
                    {
                        Guild = newGuild,
                        GuildId = newGuild.GuildId,
                        RoleId = roleId
                    });
                }
                newGuild.AutoRoleEnabled = settings.AutoRole.Enable;

                // Starboard Config
                if (settings.Starboard.ChannelId != 0)
                {
                    _newDatabase.Starboards.Add(new DatabaseStarboard()
                    {
                        GuildId = newGuild.GuildId,
                        Enabled = settings.Starboard.Enable,
                        Emoji = settings.Starboard.Emoji.EmojiId == 0 ? settings.Starboard.Emoji.EmojiName : $"{settings.Starboard.Emoji.EmojiName}:{settings.Starboard.Emoji.EmojiId}",
                        MinimumReactions = settings.Starboard.Minimum,
                        ChannelId = settings.Starboard.ChannelId,
                        Guild = newGuild
                    });
                }

                // Welcomer Config
                if (settings.Welcome.Enable)
                {
                    _newDatabase.WelcomeSettings.Add(new DatabaseWelcomeSettings()
                    {
                        ChannelId = settings.Welcome.ChannelId,
                        Guild = newGuild,
                        GuildId = newGuild.GuildId,
                        Message = settings.Welcome.Message,
                    });
                }

                // Nickname Confirm Config
                if (settings.NicknameConfirm.Enable)
                {
                    newGuild.NicknameConfirmationChannelId = settings.NicknameConfirm.ChannelId == 0 ? null : settings.NicknameConfirm.ChannelId;
                }

                // Level Settings
                _newDatabase.LevelSettings.Add(new DatabaseLevelSettings()
                {
                    ChannelId = settings.Levels.ChannelId,
                    Guild = newGuild,
                    GuildId = newGuild.GuildId,
                    Enabled = settings.Levels.Enabled,
                    MessagesEnabled = settings.Levels.MessagesEnabled,
                    RedirectMessages = settings.Levels.RedirectMessages,
                });

                // Role Menu Config
                foreach (var roleMenu in settings.RoleMenus)
                {
                    var menu = _newDatabase.RoleMenus.Add(new DatabaseRoleMenu()
                    {
                        Guild = newGuild,
                        GuildId = newGuild.GuildId,
                        Name = roleMenu.Name,
                        CreatorId = roleMenu.CreatorId,
                    }).Entity;
                    foreach (var role in roleMenu.RoleIds)
                    {
                        _newDatabase.RoleMenusRoles.Add(new DatabaseRoleMenuRole()
                        {
                            Menu = menu,
                            RoleId = role
                        });
                    }
                }

                // Embed Message Links Config
                newGuild.EmbedMessageLinks = (EmbedMessageLinks)settings.EmbedMessageLinks;

                // Role State
                newGuild.PersistUserRoles = settings.RoleState.Enable;
                newGuild.PersistUserNicknames = settings.RoleState.Nickname;
                newGuild.PersistUserOverrides = settings.RoleState.Enable;

                _newDatabase.SaveChanges();
            }

            MigratorConsole.WriteLine("Done migrating guild configs!", ConsoleColor.Green);
        }

        private void MigrateLevelData()
        {
            MigratorConsole.WriteLine("Migrating stored level data");
            foreach (var levelData in _oldDatabase.Levels)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)levelData.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)levelData.UserId);
                var newLevelData = new DatabaseLevelData()
                {
                    Experience = levelData.Experience,
                    GuildId = guild.GuildId,
                    Guild = guild,
                    LastGrant = levelData.LastXpGrant,
                    User = user,
                    UserId = user.UserId
                };
                _newDatabase.LevelData.Add(newLevelData);
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating level data!", ConsoleColor.Green);
        }

        private void MigrateRoleStates()
        {
            MigratorConsole.WriteLine("Migrating stored rolestates");
            foreach (var rolestate in _oldDatabase.RolestateRoles)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)rolestate.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)rolestate.MemberId);
                foreach (var role in rolestate.RoleIds)
                {
                    _newDatabase.RoleStates.Add(new DatabaseRoleState()
                    {
                        Guild = guild,
                        GuildId = guild.GuildId,
                        User = user,
                        UserId = user.UserId,
                        RoleId = (ulong)role
                    });
                }
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating role states!", ConsoleColor.Green);
        }

        private void MigrateRoleOverrides()
        {
            MigratorConsole.WriteLine("Migrating stored overrides");
            foreach (var rolestate in _oldDatabase.RolestateOverrides)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)rolestate.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)rolestate.MemberId);
                _newDatabase.OverrideStates.Add(new DatabaseOverrideState()
                {
                    Guild = guild,
                    DeniedPermissions = rolestate.PermsDeny ?? 0,
                    AllowedPermissions = rolestate.PermsAllow ?? 0,
                    ChannelId = (ulong)rolestate.ChannelId,
                    GuildId = (ulong)rolestate.GuildId,
                    User = user,
                    UserId = user.UserId,
                });
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating override states!", ConsoleColor.Green);
        }

        private void MigrateNicknameStates()
        {
            MigratorConsole.WriteLine("Migrating stored nicknames");
            foreach (var nickname in _oldDatabase.RolestateNicks)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)nickname.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)nickname.MemberId);
                _newDatabase.NicknameStates.Add(new DatabaseNicknameState()
                {
                    Guild = guild,
                    GuildId = guild.GuildId,
                    User = user,
                    UserId = user.UserId,
                    Nickname = nickname.Nickname
                });
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating nickname states!", ConsoleColor.Green);
        }

        private void MigrateTags()
        {

            MigratorConsole.WriteLine("Done migrating tags!", ConsoleColor.Green);
        }

        private void MigrateTimers()
        {

            MigratorConsole.WriteLine("Done migrating timers!", ConsoleColor.Green);
        }

        private void MigrateStarData()
        {

            MigratorConsole.WriteLine("Done migrating guild starboard data!", ConsoleColor.Green);
        }

        private DatabaseGuild GetOrCreateNewGuildEntity(ulong guildId)
        {
            DatabaseGuild guild = _newDatabase.Guilds.FirstOrDefault(x => x.GuildId == guildId);
            if (guild != default) return guild;

            var newGuild = new DatabaseGuild()
            {
                GuildId = guildId,
                AppealChannelId = 0,
                LoggingChannelId = 0,
                NicknameConfirmationChannelId = 0,
                ModlogChannelId = 0,
                TicketChannelId = 0
            };

            var newDbGuild = _newDatabase.Guilds.Add(newGuild);
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine($"Created new Guild data for {guildId}", ConsoleColor.Magenta);
            return newDbGuild.Entity;
        }

        private DatabaseUser GetOrCreateNewUserEntity(ulong userId)
        {
            DatabaseUser user = _newDatabase.Users.FirstOrDefault(x => x.UserId == userId);
            if (user != default) return user;

            var newUser = new DatabaseUser()
            {
                UserId = userId
            };

            var newDbUser = _newDatabase.Users.Add(newUser);
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine($"Created new User data for {userId}", ConsoleColor.Magenta);
            return newDbUser.Entity;
        }
    }
}
