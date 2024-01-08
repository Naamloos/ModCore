using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModCore.Common.Database.Migrations
{
    /// <inheritdoc />
    public partial class ShardedTimers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "shard_id",
                table: "mcore_timers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shard_id",
                table: "mcore_timers");
        }
    }
}
