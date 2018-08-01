using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ModCore.Migrations
{
    public partial class DataAnnotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "command_qualified",
                table: "mcore_cmd_state");

            migrationBuilder.DropIndex(
                name: "index_id",
                table: "mcore_cmd_state");

            migrationBuilder.AlterColumn<short>(
                name: "id",
                table: "mcore_cmd_state",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_mcore_cmd_state",
                table: "mcore_cmd_state",
                column: "command_qualified");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mcore_cmd_state",
                table: "mcore_cmd_state");

            migrationBuilder.AlterColumn<short>(
                name: "id",
                table: "mcore_cmd_state",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "command_qualified",
                table: "mcore_cmd_state",
                column: "command_qualified");

            migrationBuilder.CreateIndex(
                name: "index_id",
                table: "mcore_cmd_state",
                column: "id",
                unique: true);
        }
    }
}
