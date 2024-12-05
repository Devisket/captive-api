using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckInventoryDetails_SchemaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "check_inventory_detail");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "check_inventory_detail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "check_inventory_detail");

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "check_inventory_detail",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
