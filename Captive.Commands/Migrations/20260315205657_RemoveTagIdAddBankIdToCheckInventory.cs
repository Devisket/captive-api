using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTagIdAddBankIdToCheckInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory");

            migrationBuilder.DropIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "check_inventory_detail");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "check_inventory",
                newName: "BankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "check_inventory",
                newName: "TagId");

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
