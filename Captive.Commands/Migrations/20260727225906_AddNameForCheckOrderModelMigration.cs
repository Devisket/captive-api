using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddNameForCheckOrderModelMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName3",
                table: "floating_check_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName4",
                table: "floating_check_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName1",
                table: "check_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName2",
                table: "check_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName3",
                table: "check_orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName4",
                table: "check_orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName3",
                table: "floating_check_orders");

            migrationBuilder.DropColumn(
                name: "AccountName4",
                table: "floating_check_orders");

            migrationBuilder.DropColumn(
                name: "AccountName1",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "AccountName2",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "AccountName3",
                table: "check_orders");

            migrationBuilder.DropColumn(
                name: "AccountName4",
                table: "check_orders");
        }
    }
}
