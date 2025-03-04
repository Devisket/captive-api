using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCheckValidationMigratonScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_check_validation_CheckValidationId",
                table: "tag");

            migrationBuilder.DropTable(
                name: "check_validation");

            migrationBuilder.DropIndex(
                name: "IX_tag_CheckValidationId",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration");

            migrationBuilder.DropIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "CheckInventoryId",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "CheckValidationId",
                table: "product_configuration");

            migrationBuilder.RenameColumn(
                name: "CheckValidationId",
                table: "tag",
                newName: "BankId");

            migrationBuilder.AddColumn<bool>(
                name: "SearchByAccount",
                table: "tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByBranch",
                table: "tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByFormCheck",
                table: "tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SearchByProduct",
                table: "tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnable",
                table: "check_inventory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "SearchByAccount",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "SearchByBranch",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "SearchByFormCheck",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "SearchByProduct",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "check_inventory");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "tag",
                newName: "CheckValidationId");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckInventoryId",
                table: "tag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CheckValidationId",
                table: "product_configuration",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "check_validation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_validation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_validation_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tag_CheckValidationId",
                table: "tag",
                column: "CheckValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                column: "TagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_validation_BankInfoId",
                table: "check_validation",
                column: "BankInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_check_validation_CheckValidationId",
                table: "product_configuration",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tag_check_validation_CheckValidationId",
                table: "tag",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
