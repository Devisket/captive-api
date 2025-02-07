using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddDeleteCascadeForCheckOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_detail_check_inventory_CheckInventoryId",
                table: "check_inventory_detail");

            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_detail_check_orders_CheckOrderId",
                table: "check_inventory_detail");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_detail_check_inventory_CheckInventoryId",
                table: "check_inventory_detail",
                column: "CheckInventoryId",
                principalTable: "check_inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_detail_check_orders_CheckOrderId",
                table: "check_inventory_detail",
                column: "CheckOrderId",
                principalTable: "check_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_detail_check_inventory_CheckInventoryId",
                table: "check_inventory_detail");

            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_detail_check_orders_CheckOrderId",
                table: "check_inventory_detail");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_detail_check_inventory_CheckInventoryId",
                table: "check_inventory_detail",
                column: "CheckInventoryId",
                principalTable: "check_inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_detail_check_orders_CheckOrderId",
                table: "check_inventory_detail",
                column: "CheckOrderId",
                principalTable: "check_orders",
                principalColumn: "Id");
        }
    }
}
