using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckInventoryDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "StartingNumber",
                table: "check_inventory_detail",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "EndingNumber",
                table: "check_inventory_detail",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StartingNumber",
                table: "check_inventory_detail",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "EndingNumber",
                table: "check_inventory_detail",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
