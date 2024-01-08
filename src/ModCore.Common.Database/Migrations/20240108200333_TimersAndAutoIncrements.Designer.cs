﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModCore.Common.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    [DbContext(typeof(DatabaseConnection))]
    [Migration("20240108200333_TimersAndAutoIncrements")]
    partial class TimersAndAutoIncrements
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseAutoRole", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("role_id");

                    b.HasKey("GuildId", "RoleId");

                    b.ToTable("mcore_autorole");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseBanAppeal", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("AppealContent")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("appeal_content");

                    b.HasKey("UserId", "GuildId");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_appeal");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseGuild", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal?>("AppealChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("appeal_channel_id");

                    b.Property<decimal?>("LoggingChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("logging_channel_id");

                    b.Property<decimal?>("ModlogChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("modlog_channel_id");

                    b.Property<decimal?>("NicknameConfirmationChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("nick_confirm_channel_id");

                    b.Property<decimal?>("TicketChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("ticket_channel_id");

                    b.HasKey("GuildId");

                    b.ToTable("mcore_guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseInfraction", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<decimal>("ResponsibleModerator")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("responsible_moderator_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("infraction_type");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<bool>("UserNotified")
                        .HasColumnType("boolean")
                        .HasColumnName("user_was_notified");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_infraction");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLevelData", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<long>("Experience")
                        .HasColumnType("bigint")
                        .HasColumnName("experience");

                    b.Property<DateTimeOffset>("LastGrant")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_xp_grant");

                    b.HasKey("GuildId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("mcore_leveldata");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLoggerSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<bool>("LogAvatars")
                        .HasColumnType("boolean")
                        .HasColumnName("log_avatars");

                    b.Property<bool>("LogChannels")
                        .HasColumnType("boolean")
                        .HasColumnName("log_channels");

                    b.Property<bool>("LogGuildEdits")
                        .HasColumnType("boolean")
                        .HasColumnName("log_guild_edit");

                    b.Property<bool>("LogInvites")
                        .HasColumnType("boolean")
                        .HasColumnName("log_invites");

                    b.Property<bool>("LogJoins")
                        .HasColumnType("boolean")
                        .HasColumnName("log_joins");

                    b.Property<bool>("LogMessageEdits")
                        .HasColumnType("boolean")
                        .HasColumnName("log_message_edit");

                    b.Property<bool>("LogNicknames")
                        .HasColumnType("boolean")
                        .HasColumnName("log_nicknames");

                    b.Property<bool>("LogRoleAssignment")
                        .HasColumnType("boolean")
                        .HasColumnName("log_role_assign");

                    b.Property<bool>("LogRoleEdits")
                        .HasColumnType("boolean")
                        .HasColumnName("log_role_edit");

                    b.Property<decimal>("LoggerChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("logger_channel_id");

                    b.HasKey("GuildId");

                    b.ToTable("mcore_logger_settings");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseNicknameState", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nickname");

                    b.HasKey("GuildId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("mcore_nickname_state");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseOverrideState", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<long>("AllowedPermissions")
                        .HasColumnType("bigint")
                        .HasColumnName("allowed");

                    b.Property<long>("DeniedPermissions")
                        .HasColumnType("bigint")
                        .HasColumnName("denied");

                    b.HasKey("GuildId", "UserId", "ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("mcore_override_state");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleMenu", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("Name")
                        .HasMaxLength(30)
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_rolemenu");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleMenuRole", b =>
                {
                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("role_id");

                    b.Property<decimal>("MenuId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("menu_id");

                    b.HasKey("RoleId", "MenuId");

                    b.HasIndex("MenuId");

                    b.ToTable("mcore_rolemenu_role");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleState", b =>
                {
                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("role_id");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.HasKey("RoleId", "UserId", "GuildId");

                    b.HasIndex("GuildId");

                    b.HasIndex("UserId");

                    b.ToTable("mcore_role_state");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseStarboard", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<string>("Emoji")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("emoji");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean")
                        .HasColumnName("enabled");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<int>("MinimumReactions")
                        .HasColumnType("integer")
                        .HasColumnName("minimum_reactions");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_starboard");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseStarboardItem", b =>
                {
                    b.Property<decimal>("StarboardId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("starboard_id");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("message_id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<decimal>("AuthorId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("author_id");

                    b.Property<decimal>("BoardMessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("board_message_id");

                    b.Property<decimal>("StarAmount")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("star_amount");

                    b.HasKey("StarboardId", "MessageId", "ChannelId");

                    b.HasIndex("AuthorId");

                    b.ToTable("mcore_starboard_item");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTag", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<decimal>("AuthorId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("author_id");

                    b.Property<decimal?>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modifed_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("character varying(35)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_tag");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTagHistory", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("content");

                    b.Property<decimal>("TagId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("tag_id");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("mcore_tag_history");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTicket", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<decimal>("AuthorId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("author_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal?>("TicketThreadId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("ticket_thread_id");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GuildId");

                    b.ToTable("mcore_ticket");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTimer", b =>
                {
                    b.Property<decimal>("TimerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("timer_id");

                    b.Property<string>("Data")
                        .HasColumnType("jsonb")
                        .HasColumnName("data");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTimeOffset>("TriggersAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("trigger_at");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("TimerId");

                    b.ToTable("mcore_timers");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseUser", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("UserId");

                    b.ToTable("mcore_user");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseWelcomeSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<string>("ImageB64")
                        .HasColumnType("text")
                        .HasColumnName("image_b64");

                    b.Property<int>("ImageHeight")
                        .HasColumnType("integer")
                        .HasColumnName("height");

                    b.Property<int>("ImageWidth")
                        .HasColumnType("integer")
                        .HasColumnName("width");

                    b.Property<int>("ImageX")
                        .HasColumnType("integer")
                        .HasColumnName("x");

                    b.Property<int>("ImageY")
                        .HasColumnType("integer")
                        .HasColumnName("y");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_id");

                    b.Property<int>("Shape")
                        .HasColumnType("integer")
                        .HasColumnName("shape");

                    b.HasKey("GuildId");

                    b.ToTable("mcore_welcomer");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseAutoRole", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("AutoRoles")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseBanAppeal", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("BanAppeals")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "User")
                        .WithMany("BanAppeals")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseInfraction", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("Infractions")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLevelData", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("LevelData")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "User")
                        .WithMany("LevelData")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseLoggerSettings", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithOne("LoggerSettings")
                        .HasForeignKey("ModCore.Common.Database.Entities.DatabaseLoggerSettings", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseNicknameState", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("NicknameStates")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "User")
                        .WithMany("NicknameStates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseOverrideState", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("OverrideStates")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "User")
                        .WithMany("OverrideStates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleMenu", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("RoleMenus")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleMenuRole", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseRoleMenu", "Menu")
                        .WithMany("Roles")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Menu");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleState", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("RoleStates")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "User")
                        .WithMany("RoleStates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseStarboard", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("Starboards")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseStarboardItem", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "Author")
                        .WithMany("StarboardItems")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseStarboard", "Starboard")
                        .WithMany("Items")
                        .HasForeignKey("StarboardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Starboard");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTag", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "Author")
                        .WithMany("Tags")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("Tags")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTagHistory", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseTag", "Tag")
                        .WithMany("History")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTicket", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseUser", "Author")
                        .WithMany("Tickets")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithMany("Tickets")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseWelcomeSettings", b =>
                {
                    b.HasOne("ModCore.Common.Database.Entities.DatabaseGuild", "Guild")
                        .WithOne("WelcomeSettings")
                        .HasForeignKey("ModCore.Common.Database.Entities.DatabaseWelcomeSettings", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseGuild", b =>
                {
                    b.Navigation("AutoRoles");

                    b.Navigation("BanAppeals");

                    b.Navigation("Infractions");

                    b.Navigation("LevelData");

                    b.Navigation("LoggerSettings")
                        .IsRequired();

                    b.Navigation("NicknameStates");

                    b.Navigation("OverrideStates");

                    b.Navigation("RoleMenus");

                    b.Navigation("RoleStates");

                    b.Navigation("Starboards");

                    b.Navigation("Tags");

                    b.Navigation("Tickets");

                    b.Navigation("WelcomeSettings")
                        .IsRequired();
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseRoleMenu", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseStarboard", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseTag", b =>
                {
                    b.Navigation("History");
                });

            modelBuilder.Entity("ModCore.Common.Database.Entities.DatabaseUser", b =>
                {
                    b.Navigation("BanAppeals");

                    b.Navigation("LevelData");

                    b.Navigation("NicknameStates");

                    b.Navigation("OverrideStates");

                    b.Navigation("RoleStates");

                    b.Navigation("StarboardItems");

                    b.Navigation("Tags");

                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
