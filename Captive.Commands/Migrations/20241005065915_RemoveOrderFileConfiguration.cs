using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderFileConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_configuration_order_file_configuration_OrderFileConfigurationId",
                table: "product_configuration");

            migrationBuilder.DropTable(
                name: "order_file_configuration");

            migrationBuilder.DropIndex(
                name: "IX_product_configuration_OrderFileConfigurationId",
                table: "product_configuration");

            migrationBuilder.DropColumn(
                name: "OrderFileConfigurationId",
                table: "product_configuration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderFileConfigurationId",
                table: "product_configuration",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "order_file_configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigurationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_file_configuration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_file_configuration_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_OrderFileConfigurationId",
                table: "product_configuration",
                column: "OrderFileConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_order_file_configuration_BankInfoId",
                table: "order_file_configuration",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_order_file_configuration_Name",
                table: "order_file_configuration",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_product_configuration_order_file_configuration_OrderFileConfigurationId",
                table: "product_configuration",
                column: "OrderFileConfigurationId",
                principalTable: "order_file_configuration",
                principalColumn: "Id");
        }
    }
}
