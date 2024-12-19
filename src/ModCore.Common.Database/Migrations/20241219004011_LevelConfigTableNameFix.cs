using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class LevelConfigTableNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelSettings_mcore_guild_guild_id",
                table: "LevelSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LevelSettings",
                table: "LevelSettings");

            migrationBuilder.RenameTable(
                name: "LevelSettings",
                newName: "mcore_levelsettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mcore_levelsettings",
                table: "mcore_levelsettings",
                column: "guild_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mcore_levelsettings_mcore_guild_guild_id",
                table: "mcore_levelsettings",
                column: "guild_id",
                principalTable: "mcore_guild",
                principalColumn: "guild_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mcore_levelsettings_mcore_guild_guild_id",
                table: "mcore_levelsettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mcore_levelsettings",
                table: "mcore_levelsettings");

            migrationBuilder.RenameTable(
                name: "mcore_levelsettings",
                newName: "LevelSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LevelSettings",
                table: "LevelSettings",
                column: "guild_id");

            migrationBuilder.AddForeignKey(
                name: "FK_LevelSettings_mcore_guild_guild_id",
                table: "LevelSettings",
                column: "guild_id",
                principalTable: "mcore_guild",
                principalColumn: "guild_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
