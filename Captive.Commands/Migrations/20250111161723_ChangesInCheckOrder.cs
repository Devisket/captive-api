using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInCheckOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "InputEnable",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "check_orders");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "floating_check_orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "floating_check_orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "floating_check_orders");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "floating_check_orders");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "check_orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "InputEnable",
                table: "check_orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "check_orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
