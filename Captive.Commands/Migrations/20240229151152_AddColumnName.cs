using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileInitial",
                table: "form_checks",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "BatchName",
                table: "batch_files",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "batch_files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "banks_info",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileInitial",
                table: "form_checks");

            migrationBuilder.DropColumn(
                name: "BatchName",
                table: "batch_files");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "batch_files");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "banks_info");
        }
    }
}
