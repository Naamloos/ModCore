using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ModCore.Migrations
{
    public partial class GuildConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guild_config",
                columns: table => new
                {
                    guild_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prefix = table.Column<string>(type: "text", nullable: true),
                    mute_role_id = table.Column<long>(type: "bigint", nullable: false),
                    spelling_helper_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    log_channel_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_config", x => x.guild_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_config");
        }
    }
}
