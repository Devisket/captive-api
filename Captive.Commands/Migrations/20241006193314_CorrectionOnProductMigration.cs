using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionOnProductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_form_checks_product_type_ProductId",
                table: "form_checks");

            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_product_type_ProductId",
                table: "product_configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_product_type_banks_info_BankInfoId",
                table: "product_type");

            migrationBuilder.DropForeignKey(
                name: "FK_product_type_tag_TagId",
                table: "product_type");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_type",
                table: "product_type");

            migrationBuilder.RenameTable(
                name: "product_type",
                newName: "products");

            migrationBuilder.RenameIndex(
                name: "IX_product_type_TagId",
                table: "products",
                newName: "IX_products_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_product_type_BankInfoId",
                table: "products",
                newName: "IX_products_BankInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_checks_products_ProductId",
                table: "form_checks",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_products_ProductId",
                table: "product_configuration",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_banks_info_BankInfoId",
                table: "products",
                column: "BankInfoId",
                principalTable: "banks_info",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_tag_TagId",
                table: "products",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_form_checks_products_ProductId",
                table: "form_checks");

            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_products_ProductId",
                table: "product_configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_products_banks_info_BankInfoId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tag_TagId",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "product_type");

            migrationBuilder.RenameIndex(
                name: "IX_products_TagId",
                table: "product_type",
                newName: "IX_product_type_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_products_BankInfoId",
                table: "product_type",
                newName: "IX_product_type_BankInfoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_type",
                table: "product_type",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_checks_product_type_ProductId",
                table: "form_checks",
                column: "ProductId",
                principalTable: "product_type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_product_type_ProductId",
                table: "product_configuration",
                column: "ProductId",
                principalTable: "product_type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_type_banks_info_BankInfoId",
                table: "product_type",
                column: "BankInfoId",
                principalTable: "banks_info",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_type_tag_TagId",
                table: "product_type",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id");
        }
    }
}
