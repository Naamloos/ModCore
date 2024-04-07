using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class guild_last_seen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_seen_at",
                table: "mcore_guild",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_seen_at",
                table: "mcore_guild");
        }
    }
}
