using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Migrations
{
    public partial class UpdateTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "mcore_tags_channel_id_tag_name_key",
                table: "mcore_tags");

            migrationBuilder.AddColumn<long>(
                name: "guild_id",
                table: "mcore_tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "mcore_tags_guild_id_channel_id_name_key",
                table: "mcore_tags",
                columns: new[] { "guild_id", "channel_id", "tagname" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "mcore_tags_guild_id_channel_id_name_key",
                table: "mcore_tags");

            migrationBuilder.DropColumn(
                name: "guild_id",
                table: "mcore_tags");

            migrationBuilder.CreateIndex(
                name: "mcore_tags_channel_id_tag_name_key",
                table: "mcore_tags",
                columns: new[] { "channel_id", "tagname" },
                unique: true);
        }
    }
}
