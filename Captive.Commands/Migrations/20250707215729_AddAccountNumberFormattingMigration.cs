using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountNumberFormattingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumberFormat",
                table: "banks_info",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "(\\w{3})(\\w{6})(\\w{3})");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumberFormat",
                table: "banks_info");
        }
    }
}
