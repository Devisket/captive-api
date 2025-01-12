using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationShipIntoFloatingCheckOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_floating_check_orders_OrderFileId",
                table: "floating_check_orders",
                column: "OrderFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_floating_check_orders_order_files_OrderFileId",
                table: "floating_check_orders",
                column: "OrderFileId",
                principalTable: "order_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_floating_check_orders_order_files_OrderFileId",
                table: "floating_check_orders");

            migrationBuilder.DropIndex(
                name: "IX_floating_check_orders_OrderFileId",
                table: "floating_check_orders");
        }
    }
}
