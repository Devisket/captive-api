using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckInventoryDetails_MultipleCheckOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_check_inventory_detail_CheckOrderId",
                table: "check_inventory_detail");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_detail_CheckOrderId",
                table: "check_inventory_detail",
                column: "CheckOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_check_inventory_detail_CheckOrderId",
                table: "check_inventory_detail");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_detail_CheckOrderId",
                table: "check_inventory_detail",
                column: "CheckOrderId",
                unique: true,
                filter: "[CheckOrderId] IS NOT NULL");
        }
    }
}
