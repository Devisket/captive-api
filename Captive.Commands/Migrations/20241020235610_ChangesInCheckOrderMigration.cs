using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInCheckOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_check_orders_form_checks_FormCheckId",
                table: "check_orders");

            migrationBuilder.DropIndex(
                name: "IX_check_orders_FormCheckId",
                table: "check_orders");

            migrationBuilder.AlterColumn<Guid>(
                name: "FormCheckId",
                table: "check_orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FormChecksId",
                table: "check_orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "check_orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "check_orders");

            migrationBuilder.AlterColumn<Guid>(
                name: "FormCheckId",
                table: "check_orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_FormCheckId",
                table: "check_orders",
                column: "FormCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_check_orders_form_checks_FormCheckId",
                table: "check_orders",
                column: "FormCheckId",
                principalTable: "form_checks",
                principalColumn: "Id");
        }
    }
}
