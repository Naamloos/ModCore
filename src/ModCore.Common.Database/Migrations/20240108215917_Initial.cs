using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mcore_guild",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    logging_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    modlog_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    ticket_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    appeal_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    nick_confirm_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_guild", x => x.guild_id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_timers",
                columns: table => new
                {
                    timer_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    shard_id = table.Column<int>(type: "integer", nullable: false),
                    trigger_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_timers", x => x.timer_id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_user",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_autorole",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_autorole", x => new { x.guild_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_mcore_autorole_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_infraction",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    responsible_moderator_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    user_was_notified = table.Column<bool>(type: "boolean", nullable: false),
                    infraction_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_infraction", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_infraction_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_logger_settings",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    logger_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    log_joins = table.Column<bool>(type: "boolean", nullable: false),
                    log_message_edit = table.Column<bool>(type: "boolean", nullable: false),
                    log_nicknames = table.Column<bool>(type: "boolean", nullable: false),
                    log_avatars = table.Column<bool>(type: "boolean", nullable: false),
                    log_invites = table.Column<bool>(type: "boolean", nullable: false),
                    log_role_assign = table.Column<bool>(type: "boolean", nullable: false),
                    log_channels = table.Column<bool>(type: "boolean", nullable: false),
                    log_guild_edit = table.Column<bool>(type: "boolean", nullable: false),
                    log_role_edit = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_logger_settings", x => x.guild_id);
                    table.ForeignKey(
                        name: "FK_mcore_logger_settings_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_rolemenu",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<decimal>(type: "numeric(20,0)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_rolemenu", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_rolemenu_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_starboard",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    minimum_reactions = table.Column<int>(type: "integer", nullable: false),
                    emoji = table.Column<string>(type: "text", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_starboard", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_starboard_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_welcomer",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    message_id = table.Column<string>(type: "text", nullable: false),
                    image_b64 = table.Column<string>(type: "text", nullable: true),
                    x = table.Column<int>(type: "integer", nullable: false),
                    y = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    shape = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_welcomer", x => x.guild_id);
                    table.ForeignKey(
                        name: "FK_mcore_welcomer_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_appeal",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    appeal_content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_appeal", x => new { x.user_id, x.guild_id });
                    table.ForeignKey(
                        name: "FK_mcore_appeal_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_appeal_mcore_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_leveldata",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    experience = table.Column<long>(type: "bigint", nullable: false),
                    last_xp_grant = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_leveldata", x => new { x.guild_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_mcore_leveldata_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_leveldata_mcore_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_nickname_state",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_nickname_state", x => new { x.guild_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_mcore_nickname_state_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_nickname_state_mcore_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_override_state",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    allowed = table.Column<long>(type: "bigint", nullable: false),
                    denied = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_override_state", x => new { x.guild_id, x.user_id, x.channel_id });
                    table.ForeignKey(
                        name: "FK_mcore_override_state_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_override_state_mcore_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_role_state",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_role_state", x => new { x.role_id, x.user_id, x.guild_id });
                    table.ForeignKey(
                        name: "FK_mcore_role_state_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_role_state_mcore_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_tag",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    modifed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_tag", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_tag_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_tag_mcore_user_author_id",
                        column: x => x.author_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_ticket",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    ticket_thread_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_ticket", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_ticket_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_ticket_mcore_user_author_id",
                        column: x => x.author_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_rolemenu_role",
                columns: table => new
                {
                    menu_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_rolemenu_role", x => new { x.role_id, x.menu_id });
                    table.ForeignKey(
                        name: "FK_mcore_rolemenu_role_mcore_rolemenu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "mcore_rolemenu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_starboard_item",
                columns: table => new
                {
                    starboard_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    message_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    board_message_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    star_amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_starboard_item", x => new { x.starboard_id, x.message_id, x.channel_id });
                    table.ForeignKey(
                        name: "FK_mcore_starboard_item_mcore_starboard_starboard_id",
                        column: x => x.starboard_id,
                        principalTable: "mcore_starboard",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mcore_starboard_item_mcore_user_author_id",
                        column: x => x.author_id,
                        principalTable: "mcore_user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcore_tag_history",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    tag_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_tag_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_mcore_tag_history_mcore_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "mcore_tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mcore_appeal_guild_id",
                table: "mcore_appeal",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_infraction_guild_id",
                table: "mcore_infraction",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_leveldata_user_id",
                table: "mcore_leveldata",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_nickname_state_user_id",
                table: "mcore_nickname_state",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_override_state_user_id",
                table: "mcore_override_state",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_role_state_guild_id",
                table: "mcore_role_state",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_role_state_user_id",
                table: "mcore_role_state",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_rolemenu_guild_id",
                table: "mcore_rolemenu",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_rolemenu_role_menu_id",
                table: "mcore_rolemenu_role",
                column: "menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_starboard_guild_id",
                table: "mcore_starboard",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_starboard_item_author_id",
                table: "mcore_starboard_item",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_tag_author_id",
                table: "mcore_tag",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_tag_guild_id",
                table: "mcore_tag",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_tag_history_tag_id",
                table: "mcore_tag_history",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_ticket_author_id",
                table: "mcore_ticket",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_mcore_ticket_guild_id",
                table: "mcore_ticket",
                column: "guild_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mcore_appeal");

            migrationBuilder.DropTable(
                name: "mcore_autorole");

            migrationBuilder.DropTable(
                name: "mcore_infraction");

            migrationBuilder.DropTable(
                name: "mcore_leveldata");

            migrationBuilder.DropTable(
                name: "mcore_logger_settings");

            migrationBuilder.DropTable(
                name: "mcore_nickname_state");

            migrationBuilder.DropTable(
                name: "mcore_override_state");

            migrationBuilder.DropTable(
                name: "mcore_role_state");

            migrationBuilder.DropTable(
                name: "mcore_rolemenu_role");

            migrationBuilder.DropTable(
                name: "mcore_starboard_item");

            migrationBuilder.DropTable(
                name: "mcore_tag_history");

            migrationBuilder.DropTable(
                name: "mcore_ticket");

            migrationBuilder.DropTable(
                name: "mcore_timers");

            migrationBuilder.DropTable(
                name: "mcore_welcomer");

            migrationBuilder.DropTable(
                name: "mcore_rolemenu");

            migrationBuilder.DropTable(
                name: "mcore_starboard");

            migrationBuilder.DropTable(
                name: "mcore_tag");

            migrationBuilder.DropTable(
                name: "mcore_guild");

            migrationBuilder.DropTable(
                name: "mcore_user");
        }
    }
}
