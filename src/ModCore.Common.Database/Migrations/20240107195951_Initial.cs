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
                    logging_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mcore_guild", x => x.guild_id);
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
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mcore_leveldata");

            migrationBuilder.DropTable(
                name: "mcore_guild");
        }
    }
}
