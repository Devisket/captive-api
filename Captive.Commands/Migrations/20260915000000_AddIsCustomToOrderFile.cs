using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCustomToOrderFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "order_files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "order_files");
        }
    }
}
