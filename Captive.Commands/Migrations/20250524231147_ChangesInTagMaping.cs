using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInTagMaping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "tag_mapping");

            migrationBuilder.DropColumn(
                name: "FormCheckId",
                table: "tag_mapping");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "tag_mapping");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tag_mapping",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TagMappingData",
                table: "tag_mapping",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "tag_mapping",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tag_mapping");

            migrationBuilder.DropColumn(
                name: "TagMappingData",
                table: "tag_mapping");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "tag_mapping");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FormCheckId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "tag_mapping",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
