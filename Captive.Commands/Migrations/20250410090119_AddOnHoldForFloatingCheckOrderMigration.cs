using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddOnHoldForFloatingCheckOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnHold",
                table: "floating_check_orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnHold",
                table: "floating_check_orders");
        }
    }
}
