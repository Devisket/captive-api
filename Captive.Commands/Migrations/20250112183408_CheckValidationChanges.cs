using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class CheckValidationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "ValidateByBranch",
                table: "check_validation");

            migrationBuilder.DropColumn(
                name: "ValidateByFormCheck",
                table: "check_validation");

            migrationBuilder.DropColumn(
                name: "ValidateByProduct",
                table: "check_validation");

            migrationBuilder.DropColumn(
                name: "ValidationType",
                table: "check_validation");

            migrationBuilder.RenameColumn(
                name: "LastSeriesNumber",
                table: "check_inventory",
                newName: "WarningSeries");

            migrationBuilder.RenameColumn(
                name: "CheckValidationId",
                table: "check_inventory",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_check_inventory_CheckValidationId",
                table: "check_inventory",
                newName: "IX_check_inventory_TagId");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckInventoryId",
                table: "tag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "isDefaultTag",
                table: "tag",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EndingNumber",
                table: "check_inventory_detail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartingNumber",
                table: "check_inventory_detail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPadding",
                table: "check_inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartingSeries",
                table: "check_inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isRepeating",
                table: "check_inventory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_inventory_tag_TagId",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "CheckInventoryId",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "isDefaultTag",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "EndingNumber",
                table: "check_inventory_detail");

            migrationBuilder.DropColumn(
                name: "StartingNumber",
                table: "check_inventory_detail");

            migrationBuilder.DropColumn(
                name: "NumberOfPadding",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "StartingSeries",
                table: "check_inventory");

            migrationBuilder.DropColumn(
                name: "isRepeating",
                table: "check_inventory");

            migrationBuilder.RenameColumn(
                name: "WarningSeries",
                table: "check_inventory",
                newName: "LastSeriesNumber");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "check_inventory",
                newName: "CheckValidationId");

            migrationBuilder.RenameIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                newName: "IX_check_inventory_CheckValidationId");

            migrationBuilder.AddColumn<bool>(
                name: "ValidateByBranch",
                table: "check_validation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidateByFormCheck",
                table: "check_validation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ValidateByProduct",
                table: "check_validation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidationType",
                table: "check_validation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_check_inventory_check_validation_CheckValidationId",
                table: "check_inventory",
                column: "CheckValidationId",
                principalTable: "check_validation",
                principalColumn: "Id");
        }
    }
}
