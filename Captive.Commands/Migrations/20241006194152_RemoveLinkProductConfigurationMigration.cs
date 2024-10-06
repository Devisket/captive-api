using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLinkProductConfigurationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_product_configuration_ProductId",
                table: "product_configuration");

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_ProductId",
                table: "product_configuration",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_product_configuration_ProductId",
                table: "product_configuration");

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_ProductId",
                table: "product_configuration",
                column: "ProductId",
                unique: true);
        }
    }
}
