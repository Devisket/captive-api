using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddBrachCodeSeriesFormChecksMigraton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductSequence",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBranchCodeInSeries",
                table: "form_checks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_ProductId",
                table: "check_orders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_check_orders_products_ProductId",
                table: "check_orders",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_orders_products_ProductId",
                table: "check_orders");

            migrationBuilder.DropIndex(
                name: "IX_check_orders_ProductId",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "ProductSequence",
                table: "products");

            migrationBuilder.DropColumn(
                name: "HasBranchCodeInSeries",
                table: "form_checks");
        }
    }
}
