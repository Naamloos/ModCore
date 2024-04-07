using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "mcore_rolemenu",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<decimal>(
                name: "creator_id",
                table: "mcore_rolemenu",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "logger_channel_id",
                table: "mcore_logger_settings",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<bool>(
                name: "auto_role_enabled",
                table: "mcore_guild",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "embed_message_links_state",
                table: "mcore_guild",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "persist_user_nicknames",
                table: "mcore_guild",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "persist_user_overrides",
                table: "mcore_guild",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "persist_user_roles",
                table: "mcore_guild",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LevelSettings",
                columns: table => new
                {
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    levels_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    messages_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    redirect_messages = table.Column<bool>(type: "boolean", nullable: false),
                    message_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelSettings", x => x.guild_id);
                    table.ForeignKey(
                        name: "FK_LevelSettings_mcore_guild_guild_id",
                        column: x => x.guild_id,
                        principalTable: "mcore_guild",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelSettings");

            migrationBuilder.DropColumn(
                name: "creator_id",
                table: "mcore_rolemenu");

            migrationBuilder.DropColumn(
                name: "auto_role_enabled",
                table: "mcore_guild");

            migrationBuilder.DropColumn(
                name: "embed_message_links_state",
                table: "mcore_guild");

            migrationBuilder.DropColumn(
                name: "persist_user_nicknames",
                table: "mcore_guild");

            migrationBuilder.DropColumn(
                name: "persist_user_overrides",
                table: "mcore_guild");

            migrationBuilder.DropColumn(
                name: "persist_user_roles",
                table: "mcore_guild");

            migrationBuilder.AlterColumn<decimal>(
                name: "name",
                table: "mcore_rolemenu",
                type: "numeric(20,0)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<decimal>(
                name: "logger_channel_id",
                table: "mcore_logger_settings",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);
        }
    }
}
