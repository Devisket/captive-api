using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInProductConfigurationAndOrderFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.AlterColumn<string>(
                name: "ConfigurationType",
                table: "product_configuration",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckValidationId",
                table: "product_configuration",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "order_files",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_order_files_ProductId",
                table: "order_files",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_files_products_ProductId",
                table: "order_files",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_order_files_products_ProductId",
                table: "order_files");

            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropIndex(
                name: "IX_order_files_ProductId",
                table: "order_files");

            migrationBuilder.DropColumn(
                name: "CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "order_files");

            migrationBuilder.AlterColumn<int>(
                name: "ConfigurationType",
                table: "product_configuration",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
