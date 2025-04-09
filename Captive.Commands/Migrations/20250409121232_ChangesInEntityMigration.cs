using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInEntityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnable",
                table: "check_inventory",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "bank_branchs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "bank_branchs");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "check_inventory",
                newName: "IsEnable");
        }
    }
}
