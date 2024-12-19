using Microsoft.EntityFrameworkCore;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Timers;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase.JsonEntities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                MigratorConsole.Write("Pending migrations found for new database. Apply? (y/N): ");
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

            var sw = new Stopwatch();
            sw.Start();

            //// Guild configs must be ran first, to ensure that we create guild objects where needed.
            MigrateGuildConfigs();

            //// Then, we migrate levels and stars
            MigrateLevelData();
            MigrateStarData();
            MigrateTags();
            MigrateRoleStates();
            MigrateRoleOverrides();
            MigrateNicknameStates();
            MigrateTimers();

            sw.Stop();

            MigratorConsole.WriteLine("Done migrating v2 database to v3 database!", ConsoleColor.Green);
            MigratorConsole.WriteLine($"Migration took {sw.ElapsedMilliseconds}ms! ({sw.Elapsed.ToString()})", ConsoleColor.Cyan);
        }

        private void MigrateGuildConfigs()
        {
            foreach (var guildConfig in _oldDatabase.GuildConfig)
            {
                MigratorConsole.WriteLine($"Migrating Guild config for {guildConfig.GuildId}", ConsoleColor.Magenta);
                var newGuild = GetOrCreateNewGuildEntity((ulong)guildConfig.GuildId);

                if(newGuild == null)
                {
                    MigratorConsole.WriteLine("Guild is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(guildConfig.Settings))
                {
                    MigratorConsole.WriteLine($"Guild with ID {guildConfig.GuildId} does not contain settings. Continuing.", ConsoleColor.Red);
                    continue;
                }

                var settings = guildConfig.GetSettings();

                // Not to be migrated:
                // - LinkFilter / Invite blocker (will differ too much)
                // - selfrole/reaction role: currently deprecated / unused so no data, replaced by role menus

                // Logger
                newGuild.LoggerSettings.LogAvatars = settings.Logging.AvatarLog_Enable;
                newGuild.LoggerSettings.LogChannels = false;
                newGuild.LoggerSettings.LoggerChannelId = (ulong)settings.Logging.ChannelId;
                newGuild.LoggerSettings.LogGuildEdits = false;
                newGuild.LoggerSettings.LogInvites = settings.Logging.InviteLog_Enable;
                newGuild.LoggerSettings.LogJoins = settings.Logging.JoinLog_Enable;
                newGuild.LoggerSettings.LogMessageEdits = settings.Logging.EditLog_Enable;
                newGuild.LoggerSettings.LogNicknames = settings.Logging.NickameLog_Enable;
                newGuild.LoggerSettings.LogRoleAssignment = settings.Logging.RoleLog_Enable;
                newGuild.LoggerSettings.LogRoleEdits = settings.Logging.RoleLog_Enable;

                // Autorole
                foreach (var roleId in settings.AutoRole.RoleIds)
                {
                    // avoid doubles
                    if (_newDatabase.AutoRoles.Any(x =>
                        x.RoleId == roleId &&
                        x.GuildId == newGuild.GuildId
                    ))
                        continue;

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
                    // Create starboard if not exists
                    if (!_newDatabase.Starboards.Any(x =>
                        x.ChannelId == settings.Starboard.ChannelId &&
                        x.GuildId == newGuild.GuildId
                    ))
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
                        _newDatabase.SaveChanges();
                    }
                }

                // Welcomer Config
                if (settings.Welcome.Enable)
                {
                    newGuild.WelcomeSettings.ChannelId = settings.Welcome.ChannelId;
                    newGuild.WelcomeSettings.Enabled = settings.Welcome.Enable;
                    newGuild.WelcomeSettings.Message = settings.Welcome.Message;
                }

                // Nickname Confirm Config
                if (settings.NicknameConfirm.Enable)
                {
                    newGuild.NicknameConfirmationChannelId = settings.NicknameConfirm.ChannelId == 0 ? null : settings.NicknameConfirm.ChannelId;
                }

                // Level Settings
                newGuild.LevelSettings.ChannelId = settings.Levels.ChannelId;
                newGuild.LevelSettings.Enabled = settings.Levels.Enabled;
                newGuild.LevelSettings.MessagesEnabled = settings.Levels.MessagesEnabled;
                newGuild.LevelSettings.RedirectMessages = settings.Levels.RedirectMessages;


                // Role Menu Config
                foreach (var roleMenu in settings.RoleMenus)
                {
                    if (!_newDatabase.RoleMenus.Any(x =>
                        x.GuildId == newGuild.GuildId &&
                        x.Name == roleMenu.Name
                    ))
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
                            if (_newDatabase.RoleMenusRoles.Any(x =>
                                x.MenuId == menu.Id &&
                                x.RoleId == role
                            ))
                                continue;

                            _newDatabase.RoleMenusRoles.Add(new DatabaseRoleMenuRole()
                            {
                                Menu = menu,
                                RoleId = role
                            });
                            _newDatabase.SaveChanges();
                        }
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
            MigratorConsole.WriteLine("Migrating stored level data", ConsoleColor.Magenta);
            foreach (var levelData in _oldDatabase.Levels)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)levelData.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)levelData.UserId);

                if(guild == null || user == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (_newDatabase.LevelData.Any(x => 
                    x.UserId == user.UserId &&
                    x.GuildId == guild.GuildId
                ))
                    continue;

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
                _newDatabase.SaveChanges();
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating level data!", ConsoleColor.Green);
        }

        private void MigrateRoleStates()
        {
            MigratorConsole.WriteLine("Migrating stored rolestates", ConsoleColor.Magenta);
            foreach (var rolestate in _oldDatabase.RolestateRoles)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)rolestate.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)rolestate.MemberId);

                if(guild == null || user == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                foreach (var role in rolestate.RoleIds)
                {
                    if (_newDatabase.RoleStates.Any(x =>
                        x.UserId == user.UserId &&
                        x.RoleId == (ulong)role &&
                        x.GuildId == guild.GuildId
                    ))
                        continue;

                    _newDatabase.RoleStates.Add(new DatabaseRoleState()
                    {
                        Guild = guild,
                        GuildId = guild.GuildId,
                        User = user,
                        UserId = user.UserId,
                        RoleId = (ulong)role
                    });
                    _newDatabase.SaveChanges();
                }
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating role states!", ConsoleColor.Green);
        }

        private void MigrateRoleOverrides()
        {
            MigratorConsole.WriteLine("Migrating stored overrides", ConsoleColor.Magenta);
            foreach (var rolestate in _oldDatabase.RolestateOverrides)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)rolestate.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)rolestate.MemberId);

                if (guild == null || user == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (_newDatabase.OverrideStates.Any(x =>
                    x.UserId == user.UserId &&
                    x.GuildId == guild.GuildId &&
                    x.ChannelId == (ulong)rolestate.ChannelId
                ))
                    continue;

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
                _newDatabase.SaveChanges();
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating override states!", ConsoleColor.Green);
        }

        private void MigrateNicknameStates()
        {
            MigratorConsole.WriteLine("Migrating stored nicknames", ConsoleColor.Magenta);
            foreach (var nickname in _oldDatabase.RolestateNicks)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)nickname.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)nickname.MemberId);

                if(string.IsNullOrWhiteSpace(nickname.Nickname))
                {
                    MigratorConsole.WriteLine("Nickname is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (guild == null || user == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (_newDatabase.NicknameStates.Any(x =>
                    x.UserId == user.UserId &&
                    x.GuildId == guild.GuildId
                ))
                    continue;

                _newDatabase.NicknameStates.Add(new DatabaseNicknameState()
                {
                    Guild = guild,
                    GuildId = guild.GuildId,
                    User = user,
                    UserId = user.UserId,
                    Nickname = nickname.Nickname
                });
                _newDatabase.SaveChanges();
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating nickname states!", ConsoleColor.Green);
        }

        private void MigrateTags()
        {
            MigratorConsole.WriteLine("Migrating stored tags", ConsoleColor.Magenta);
            foreach (var tag in _oldDatabase.Tags)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)tag.GuildId);
                var usre = GetOrCreateNewUserEntity((ulong)tag.OwnerId);

                if (guild == null || usre == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (_newDatabase.Tags.Any(x =>
                    x.GuildId == guild.GuildId &&
                    x.Name == tag.Name
                ))
                    continue;

                if(string.IsNullOrWhiteSpace(tag.Contents))
                {
                    MigratorConsole.WriteLine($"Tag {tag.Name} has no content, skipping.", ConsoleColor.Red);
                    continue;
                }

                var content = tag.Contents;
                if(content.Length > 2000)
                {
                    content = content.Substring(0, 2000);
                }

                _newDatabase.Tags.Add(new DatabaseTag()
                {
                    Guild = guild,
                    GuildId = guild.GuildId,
                    Name = tag.Name,
                    AuthorId = (ulong)tag.OwnerId,
                    ChannelId = (ulong)tag.ChannelId,
                    CreatedAt = tag.CreatedAt,
                    Content = content,
                    ModifiedAt = tag.CreatedAt
                });
                _newDatabase.SaveChanges();
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating tags!", ConsoleColor.Green);
        }

        private void MigrateTimers()
        {
            MigratorConsole.WriteLine("Migrating stored timers", ConsoleColor.Magenta);
            foreach (var timer in _oldDatabase.Timers)
            {
                // types: reminder, unban. others are deprecated.
                if (timer.ActionType != TimerActionType.Reminder && timer.ActionType != TimerActionType.Unban) continue;

                var guild = GetOrCreateNewGuildEntity((ulong)timer.GuildId);

                if (guild == null)
                {
                    MigratorConsole.WriteLine("Guild or User is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                var newTimer = new DatabaseTimer()
                {
                    GuildId = guild.GuildId,
                    ShardId = 0,
                    TriggersAt = timer.DispatchAt,
                    Type = timer.ActionType == TimerActionType.Reminder ? TimerTypes.Reminder : TimerTypes.Unban
                };

                if(newTimer.Type == TimerTypes.Reminder)
                {
                    newTimer.SetData(ConvertReminderTimer(timer.GetData<TimerReminderData>(), (ulong)timer.ChannelId, (ulong)timer.UserId));
                }
                else
                {
                    newTimer.SetData(ConvertUnbanTimer(timer.GetData<TimerUnbanData>()));
                }
                _newDatabase.Timers.Add(newTimer);
                _newDatabase.SaveChanges();
            }
            _newDatabase.SaveChanges();
            MigratorConsole.WriteLine("Done migrating timers!", ConsoleColor.Green);
        }

        private UnbanTimerData ConvertUnbanTimer(TimerUnbanData oldData)
        {
            return new UnbanTimerData()
            {
                UserId = (ulong)oldData.UserId,
                DisplayName = oldData.DisplayName
            };
        }

        private ReminderTimerData ConvertReminderTimer(TimerReminderData oldData, ulong channelId, ulong userId)
        {
            return new ReminderTimerData()
            {
                ChannelId = channelId,
                CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(oldData.OriginalUnix),
                Snoozed = oldData.Snoozed,
                Text = oldData.ReminderText,
                UserId = userId
            };
        }

        private void MigrateStarData()
        {
            MigratorConsole.WriteLine("Migrating stored star data", ConsoleColor.Magenta);
            DatabaseStarboard? starboard = null;

            foreach(var starData in _oldDatabase.StarDatas)
            {
                var guild = GetOrCreateNewGuildEntity((ulong)starData.GuildId);
                var user = GetOrCreateNewUserEntity((ulong)starData.StargazerId);
                var author = GetOrCreateNewUserEntity((ulong)starData.AuthorId);

                if (guild == null || user == null || author == null)
                {
                    MigratorConsole.WriteLine("Guild, User or Author is null, skipping.", ConsoleColor.Red);
                    continue;
                }

                if (starboard == null)
                {
                    starboard = _newDatabase.Starboards.First(x => x.GuildId == guild.GuildId);
                }

                // Skip if key already exists to avoid doubles
                if (_newDatabase.StarboardItems.Any(x =>
                    x.StarboardId == starboard.Id &&
                    x.MessageId == (ulong)starData.MessageId &&
                    x.ChannelId == (ulong)starData.ChannelId &&
                    x.StargazerId == (ulong)starData.StargazerId
                ))
                    continue;

                _newDatabase.StarboardItems.Add(new DatabaseStarboardItem()
                {
                    StarboardId = starboard.Id,
                    AuthorId = (ulong)starData.AuthorId,
                    BoardMessageId = (ulong)starData.StarboardMessageId,
                    MessageId = (ulong)starData.MessageId,
                    ChannelId = (ulong)starData.ChannelId,
                    StargazerId = (ulong)starData.StargazerId,
                });
                _newDatabase.SaveChanges();
            }
            MigratorConsole.WriteLine("Done migrating guild starboard data!", ConsoleColor.Green);
        }

        private DatabaseGuild GetOrCreateNewGuildEntity(ulong guildId)
        {
            DatabaseGuild guild = _newDatabase.Guilds.FirstOrDefault(x => x.GuildId == guildId);
            if (guild != default) return guild;

            if(guildId == null || guildId == 0)
            {
                MigratorConsole.WriteLine("Guild ID is null or 0, skipping.", ConsoleColor.Red);
                return null;
            }

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

            if (userId == null || userId == 0)
            {
                MigratorConsole.WriteLine("User ID is null or 0, skipping.", ConsoleColor.Red);
                return null;
            }

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
