using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ProductConfiguraton_FK_Fix_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id");
        }
    }
}
