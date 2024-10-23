using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Batch_and_CheckOrdering_Model_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_orders_form_checks_FormChecksId",
                table: "check_orders");

            migrationBuilder.DropIndex(
                name: "IX_check_orders_FormChecksId",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "FormChecksId",
                table: "check_orders");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "batch_files",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormChecksId",
                table: "check_orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "batch_files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_FormChecksId",
                table: "check_orders",
                column: "FormChecksId");

            migrationBuilder.AddForeignKey(
                name: "FK_check_orders_form_checks_FormChecksId",
                table: "check_orders",
                column: "FormChecksId",
                principalTable: "form_checks",
                principalColumn: "Id");
        }
    }
}
