using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class StarboardFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mcore_starboard_item",
                table: "mcore_starboard_item");

            migrationBuilder.RenameColumn(
                name: "star_amount",
                table: "mcore_starboard_item",
                newName: "stargazer_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mcore_starboard_item",
                table: "mcore_starboard_item",
                columns: new[] { "starboard_id", "message_id", "channel_id", "stargazer_id" });

            migrationBuilder.CreateIndex(
                name: "IX_mcore_starboard_item_stargazer_id",
                table: "mcore_starboard_item",
                column: "stargazer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mcore_starboard_item_mcore_user_stargazer_id",
                table: "mcore_starboard_item",
                column: "stargazer_id",
                principalTable: "mcore_user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mcore_starboard_item_mcore_user_stargazer_id",
                table: "mcore_starboard_item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mcore_starboard_item",
                table: "mcore_starboard_item");

            migrationBuilder.DropIndex(
                name: "IX_mcore_starboard_item_stargazer_id",
                table: "mcore_starboard_item");

            migrationBuilder.RenameColumn(
                name: "stargazer_id",
                table: "mcore_starboard_item",
                newName: "star_amount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mcore_starboard_item",
                table: "mcore_starboard_item",
                columns: new[] { "starboard_id", "message_id", "channel_id" });
        }
    }
}
