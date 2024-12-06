using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckInventoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StarSeries",
                table: "check_inventory_detail",
                newName: "StartingSeries");

            migrationBuilder.RenameColumn(
                name: "EndSeries",
                table: "check_inventory_detail",
                newName: "EndingSeries");

            migrationBuilder.AddColumn<string>(
                name: "SeriesPatern",
                table: "check_inventory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeriesPatern",
                table: "check_inventory");

            migrationBuilder.RenameColumn(
                name: "StartingSeries",
                table: "check_inventory_detail",
                newName: "StarSeries");

            migrationBuilder.RenameColumn(
                name: "EndingSeries",
                table: "check_inventory_detail",
                newName: "EndSeries");
        }
    }
}
