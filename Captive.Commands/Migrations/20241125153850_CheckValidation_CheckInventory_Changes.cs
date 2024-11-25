using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckValidation_CheckInventory_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckInventoryId",
                table: "check_validation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "CheckInventoryId",
                table: "check_validation");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "check_inventory_detail");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId");
        }
    }
}
