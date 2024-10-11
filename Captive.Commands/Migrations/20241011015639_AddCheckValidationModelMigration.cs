using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckValidationModelMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_banks_info_BankInfoId",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "TagType",
                table: "tag");

            migrationBuilder.RenameColumn(
                name: "BankInfoId",
                table: "tag",
                newName: "CheckValidationId");

            migrationBuilder.RenameIndex(
                name: "IX_tag_BankInfoId",
                table: "tag",
                newName: "IX_tag_CheckValidationId");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "check_inventory",
                newName: "CheckValidationId");

            migrationBuilder.RenameIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                newName: "IX_check_inventory_CheckValidationId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProducTypeId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FormCheckId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FormCheckId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckValidation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckValidation", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_CheckValidation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "CheckValidation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tag_CheckValidation_CheckValidationId",
                table: "tag",
                column: "CheckValidationId",
                principalTable: "CheckValidation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_CheckValidation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_CheckValidation_CheckValidationId",
                table: "tag");

            migrationBuilder.DropTable(
                name: "CheckValidation");

            migrationBuilder.DropColumn(
                name: "FormCheckId",
                table: "tag_mapping");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "check_inventory_detail");

            migrationBuilder.DropColumn(
                name: "FormCheckId",
                table: "check_inventory_detail");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "check_inventory_detail");

            migrationBuilder.RenameColumn(
                name: "CheckValidationId",
                table: "tag",
                newName: "BankInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_tag_CheckValidationId",
                table: "tag",
                newName: "IX_tag_BankInfoId");

            migrationBuilder.RenameColumn(
                name: "CheckValidationId",
                table: "check_inventory",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory",
                newName: "IX_check_inventory_TagId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProducTypeId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagType",
                table: "tag",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tag_banks_info_BankInfoId",
                table: "tag",
                column: "BankInfoId",
                principalTable: "banks_info",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
