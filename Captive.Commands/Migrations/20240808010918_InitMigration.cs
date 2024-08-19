using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banks_info",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks_info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "batch_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    BatchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchFileStatus = table.Column<int>(type: "int", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_batch_files_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_file_configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfigurationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagType = table.Column<int>(type: "int", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tag_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_files_batch_files_BatchFileId",
                        column: x => x.BatchFileId,
                        principalTable: "batch_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bank_branchs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BRSTNCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchAddress3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchAddress4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchAddress5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MergingBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_branchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bank_branchs_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bank_branchs_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "check_inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_inventory_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_type",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_type", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_type_banks_info_BankInfoId",
                        column: x => x.BankInfoId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_type_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tag_mapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProducTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "order_file_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_file_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_file_logs_order_files_OrderFileId",
                        column: x => x.OrderFileId,
                        principalTable: "order_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "form_checks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    FileInitial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_checks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_form_checks_product_type_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product_type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_form_checks_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "product_configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigurationData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigurationType = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderFileConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_configuration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_configuration_order_file_configuration_OrderFileConfigurationId",
                        column: x => x.OrderFileConfigurationId,
                        principalTable: "order_file_configuration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_product_configuration_product_type_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product_type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "check_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BRSTN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderQuanity = table.Column<int>(type: "int", nullable: false),
                    DeliverTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InputEnable = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_orders_form_checks_FormCheckId",
                        column: x => x.FormCheckId,
                        principalTable: "form_checks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_check_orders_order_files_OrderFileId",
                        column: x => x.OrderFileId,
                        principalTable: "order_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "check_inventory_detail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StarSeries = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndSeries = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReserve = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CheckOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckInventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_inventory_detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_inventory_detail_check_inventory_CheckInventoryId",
                        column: x => x.CheckInventoryId,
                        principalTable: "check_inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_check_inventory_detail_check_orders_CheckOrderId",
                        column: x => x.CheckOrderId,
                        principalTable: "check_orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_BankInfoId",
                table: "bank_branchs",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_BRSTNCode",
                table: "bank_branchs",
                column: "BRSTNCode");

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_TagId",
                table: "bank_branchs",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_batch_files_BankInfoId",
                table: "batch_files",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_TagId",
                table: "check_inventory",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_detail_CheckInventoryId",
                table: "check_inventory_detail",
                column: "CheckInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_check_inventory_detail_CheckOrderId",
                table: "check_inventory_detail",
                column: "CheckOrderId",
                unique: true,
                filter: "[CheckOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_FormCheckId",
                table: "check_orders",
                column: "FormCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_OrderFileId",
                table: "check_orders",
                column: "OrderFileId");

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_FormType_CheckType",
                table: "form_checks",
                columns: new[] { "FormType", "CheckType" });

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_ProductId",
                table: "form_checks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_TagId",
                table: "form_checks",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_order_file_configuration_BankInfoId",
                table: "order_file_configuration",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_order_file_configuration_Name",
                table: "order_file_configuration",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_order_file_logs_OrderFileId",
                table: "order_file_logs",
                column: "OrderFileId");

            migrationBuilder.CreateIndex(
                name: "IX_order_files_BatchFileId",
                table: "order_files",
                column: "BatchFileId");

            migrationBuilder.CreateIndex(
                name: "IX_order_files_FileName",
                table: "order_files",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_OrderFileConfigurationId",
                table: "product_configuration",
                column: "OrderFileConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_product_configuration_ProductId",
                table: "product_configuration",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_type_BankInfoId",
                table: "product_type",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_product_type_TagId",
                table: "product_type",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_tag_BankInfoId",
                table: "tag",
                column: "BankInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_tag_mapping_TagId",
                table: "tag_mapping",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bank_branchs");

            migrationBuilder.DropTable(
                name: "check_inventory_detail");

            migrationBuilder.DropTable(
                name: "order_file_logs");

            migrationBuilder.DropTable(
                name: "product_configuration");

            migrationBuilder.DropTable(
                name: "seed");

            migrationBuilder.DropTable(
                name: "tag_mapping");

            migrationBuilder.DropTable(
                name: "check_inventory");

            migrationBuilder.DropTable(
                name: "check_orders");

            migrationBuilder.DropTable(
                name: "order_file_configuration");

            migrationBuilder.DropTable(
                name: "form_checks");

            migrationBuilder.DropTable(
                name: "order_files");

            migrationBuilder.DropTable(
                name: "product_type");

            migrationBuilder.DropTable(
                name: "batch_files");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "banks_info");
        }
    }
}
