using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTagAndTagMappingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tag_mapping");

            migrationBuilder.DropTable(
                name: "tag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInventoryInitiated = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsLock = table.Column<bool>(type: "bit", nullable: false),
                    SearchByAccount = table.Column<bool>(type: "bit", nullable: false),
                    SearchByBranch = table.Column<bool>(type: "bit", nullable: false),
                    SearchByFormCheck = table.Column<bool>(type: "bit", nullable: false),
                    SearchByProduct = table.Column<bool>(type: "bit", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDefaultTag = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tag_mapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TagMappingData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag_mapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tag_mapping_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tag_mapping_TagId",
                table: "tag_mapping",
                column: "TagId");
        }
    }
}
