using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "batch_jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentStep = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: null),
                    Warnings = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: null),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: null),
                    ForceProcess = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_batch_jobs_batch_files_BatchId",
                        column: x => x.BatchId,
                        principalTable: "batch_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_batch_jobs_BatchId",
                table: "batch_jobs",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "batch_jobs");
        }
    }
}
