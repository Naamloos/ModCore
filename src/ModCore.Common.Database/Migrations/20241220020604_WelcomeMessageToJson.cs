using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class WelcomeMessageToJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "image_b64",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "message_id",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "shape",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "width",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "x",
                table: "mcore_welcomer");

            migrationBuilder.DropColumn(
                name: "y",
                table: "mcore_welcomer");

            migrationBuilder.AddColumn<string>(
                name: "welcome_message_json",
                table: "mcore_welcomer",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "welcome_message_json",
                table: "mcore_welcomer");

            migrationBuilder.AddColumn<int>(
                name: "height",
                table: "mcore_welcomer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "image_b64",
                table: "mcore_welcomer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "message_id",
                table: "mcore_welcomer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "shape",
                table: "mcore_welcomer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "width",
                table: "mcore_welcomer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "x",
                table: "mcore_welcomer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "y",
                table: "mcore_welcomer",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
