using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckValidationCorrectMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_CheckValidation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckValidation_banks_info_BankInfoId",
                table: "CheckValidation");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_CheckValidation_CheckValidationId",
                table: "tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckValidation",
                table: "CheckValidation");

            migrationBuilder.RenameTable(
                name: "CheckValidation",
                newName: "check_validation");

            migrationBuilder.RenameIndex(
                name: "IX_CheckValidation_BankInfoId",
                table: "check_validation",
                newName: "IX_check_validation_BankInfoId");

            migrationBuilder.AlterColumn<string>(
                name: "ValidationType",
                table: "check_validation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_check_validation",
                table: "check_validation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_check_validation_banks_info_BankInfoId",
                table: "check_validation",
                column: "BankInfoId",
                principalTable: "banks_info",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_check_validation_banks_info_BankInfoId",
                table: "check_validation");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_check_validation_CheckValidationId",
                table: "tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_check_validation",
                table: "check_validation");

            migrationBuilder.RenameTable(
                name: "check_validation",
                newName: "CheckValidation");

            migrationBuilder.RenameIndex(
                name: "IX_check_validation_BankInfoId",
                table: "CheckValidation",
                newName: "IX_CheckValidation_BankInfoId");

            migrationBuilder.AlterColumn<int>(
                name: "ValidationType",
                table: "CheckValidation",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckValidation",
                table: "CheckValidation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_CheckValidation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "CheckValidation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckValidation_banks_info_BankInfoId",
                table: "CheckValidation",
                column: "BankInfoId",
                principalTable: "banks_info",
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
    }
}
