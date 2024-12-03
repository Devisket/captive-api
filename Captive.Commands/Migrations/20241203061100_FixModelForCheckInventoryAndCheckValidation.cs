using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class FixModelForCheckInventoryAndCheckValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductConfigurationId",
                table: "products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "check_orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "TagId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "check_inventory_detail",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductConfigurationId",
                table: "products");

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
                name: "ProductId",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "check_inventory_detail");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagId",
                table: "check_inventory_detail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
