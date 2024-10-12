using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckValidationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bank_branchs_tag_TagId",
                table: "bank_branchs");

            migrationBuilder.DropForeignKey(
                name: "FK_form_checks_tag_TagId",
                table: "form_checks");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tag_TagId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_TagId",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_form_checks_TagId",
                table: "form_checks");

            migrationBuilder.DropIndex(
                name: "IX_bank_branchs_TagId",
                table: "bank_branchs");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "form_checks");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "bank_branchs");

            migrationBuilder.RenameColumn(
                name: "ProducTypeId",
                table: "tag_mapping",
                newName: "ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "BankInfoId",
                table: "CheckValidation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "BatchFileStatus",
                table: "batch_files",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CheckValidation_BankInfoId",
                table: "CheckValidation",
                column: "BankInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckValidation_banks_info_BankInfoId",
                table: "CheckValidation",
                column: "BankInfoId",
                principalTable: "banks_info",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckValidation_banks_info_BankInfoId",
                table: "CheckValidation");

            migrationBuilder.DropIndex(
                name: "IX_CheckValidation_BankInfoId",
                table: "CheckValidation");

            migrationBuilder.DropColumn(
                name: "BankInfoId",
                table: "CheckValidation");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "tag_mapping",
                newName: "ProducTypeId");

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "form_checks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BatchFileStatus",
                table: "batch_files",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "bank_branchs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_TagId",
                table: "products",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_TagId",
                table: "form_checks",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_TagId",
                table: "bank_branchs",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_bank_branchs_tag_TagId",
                table: "bank_branchs",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_checks_tag_TagId",
                table: "form_checks",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_tag_TagId",
                table: "products",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id");
        }
    }
}
