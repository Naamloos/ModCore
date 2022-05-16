using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModCore.Migrations
{
    public partial class BaseDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mcore_bans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    ban_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_bans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_cmd_state",
                columns: table => new
                {
                    command_qualified = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_cmd_state", x => x.command_qualified);
                    table.UniqueConstraint("id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_database_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    meta_key = table.Column<string>(type: "text", nullable: false),
                    meta_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_database_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_guild_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    settings = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_guild_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_modnotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    contents = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_modnotes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_rolestate_nicks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_rolestate_nicks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_rolestate_overrides",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    perms_allow = table.Column<long>(type: "bigint", nullable: true),
                    perms_deny = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_rolestate_overrides", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_rolestate_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    role_ids = table.Column<long[]>(type: "bigint[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_rolestate_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_stars",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    stargazer_id = table.Column<long>(type: "bigint", nullable: false),
                    starboard_entry_id = table.Column<long>(type: "bigint", nullable: false),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_stars", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    tagname = table.Column<string>(type: "text", nullable: true),
                    owner_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    contents = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_timers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    dispatch_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    action_data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_timers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_userdata",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    usr_data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_userdata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mcore_warnings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<long>(type: "bigint", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    issuer_id = table.Column<long>(type: "bigint", nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    warning_text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_warnings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "mcore_database_info_meta_key_key",
                table: "mcore_database_info",
                column: "meta_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_guild_config_guild_id_key",
                table: "mcore_guild_config",
                column: "guild_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_modnotes_member_id_guild_id_key",
                table: "mcore_modnotes",
                columns: new[] { "member_id", "guild_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_rolestate_nicks_member_id_guild_id_key",
                table: "mcore_rolestate_nicks",
                columns: new[] { "member_id", "guild_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_rolestate_overrides_member_id_guild_id_channel_id_key",
                table: "mcore_rolestate_overrides",
                columns: new[] { "member_id", "guild_id", "channel_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_rolestate_roles_member_id_guild_id_key",
                table: "mcore_rolestate_roles",
                columns: new[] { "member_id", "guild_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_stars_member_id_guild_id_key",
                table: "mcore_stars",
                columns: new[] { "message_id", "channel_id", "stargazer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_tags_channel_id_tag_name_key",
                table: "mcore_tags",
                columns: new[] { "channel_id", "tagname" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mcore_userdata_user_id_key",
                table: "mcore_userdata",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mcore_bans");

            migrationBuilder.DropTable(
                name: "mcore_cmd_state");

            migrationBuilder.DropTable(
                name: "mcore_database_info");

            migrationBuilder.DropTable(
                name: "mcore_guild_config");

            migrationBuilder.DropTable(
                name: "mcore_modnotes");

            migrationBuilder.DropTable(
                name: "mcore_rolestate_nicks");

            migrationBuilder.DropTable(
                name: "mcore_rolestate_overrides");

            migrationBuilder.DropTable(
                name: "mcore_rolestate_roles");

            migrationBuilder.DropTable(
                name: "mcore_stars");

            migrationBuilder.DropTable(
                name: "mcore_tags");

            migrationBuilder.DropTable(
                name: "mcore_timers");

            migrationBuilder.DropTable(
                name: "mcore_userdata");

            migrationBuilder.DropTable(
                name: "mcore_warnings");
        }
    }
}
