using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInventoryMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonMappingData",
                table: "check_inventory");

            migrationBuilder.CreateTable(
                name: "check_inventory_mapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormCheckType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_inventory_mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_inventory_mapping_check_inventory_CheckInventoryId",
                        column: x => x.CheckInventoryId,
                        principalTable: "check_inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_mapping_BranchId",
                table: "check_inventory_mapping",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_mapping_CheckInventoryId",
                table: "check_inventory_mapping",
                column: "CheckInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_mapping_FormCheckType",
                table: "check_inventory_mapping",
                column: "FormCheckType");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_mapping_ProductId",
                table: "check_inventory_mapping",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "check_inventory_mapping");

            migrationBuilder.AddColumn<string>(
                name: "JsonMappingData",
                table: "check_inventory",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
