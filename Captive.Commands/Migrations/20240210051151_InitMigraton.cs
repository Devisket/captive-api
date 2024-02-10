using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Captive.Commands.Migrations
{
    /// <inheritdoc />
    public partial class InitMigraton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Address1 = table.Column<string>(type: "longtext", nullable: false),
                    Address2 = table.Column<string>(type: "longtext", nullable: true),
                    Address3 = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_addresses", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "banks_info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BankName = table.Column<string>(type: "longtext", nullable: false),
                    BankDescription = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks_info", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "check_accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    AccountNo = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_accounts", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_file_configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    ConfigurationData = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_file_configuration", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BatchName = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProcessDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_files", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "seed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SeedName = table.Column<string>(type: "longtext", nullable: false),
                    SeedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seed", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bank_branchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    BranchName = table.Column<string>(type: "longtext", nullable: false),
                    BranchAddress = table.Column<string>(type: "longtext", nullable: true),
                    BRSTNCode = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_branchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bank_branchs_banks_info_BankId",
                        column: x => x.BankId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "check_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_types_banks_info_BankId",
                        column: x => x.BankId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FormType = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_form_types_banks_info_BankId",
                        column: x => x.BankId,
                        principalTable: "banks_info",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CheckAccountId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: true),
                    Address1 = table.Column<string>(type: "longtext", nullable: true),
                    Address2 = table.Column<string>(type: "longtext", nullable: true),
                    AccountAddressId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_info_account_addresses_AccountAddressId",
                        column: x => x.AccountAddressId,
                        principalTable: "account_addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_account_info_check_accounts_CheckAccountId",
                        column: x => x.CheckAccountId,
                        principalTable: "check_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "check_orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    AccountNo = table.Column<string>(type: "longtext", nullable: false),
                    OrderFileId = table.Column<int>(type: "int", nullable: false),
                    BRSTN = table.Column<string>(type: "longtext", nullable: false),
                    DeliverTo = table.Column<string>(type: "longtext", nullable: true),
                    CheckAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_check_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_check_orders_check_accounts_CheckAccountId",
                        column: x => x.CheckAccountId,
                        principalTable: "check_accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_check_orders_order_files_OrderFileId",
                        column: x => x.OrderFileId,
                        principalTable: "order_files",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "form_checks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FormTypeId = table.Column<int>(type: "int", nullable: false),
                    CheckTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_checks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_form_checks_check_types_CheckTypeId",
                        column: x => x.CheckTypeId,
                        principalTable: "check_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_form_checks_form_types_FormTypeId",
                        column: x => x.FormTypeId,
                        principalTable: "form_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_account_info_AccountAddressId",
                table: "account_info",
                column: "AccountAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_info_CheckAccountId",
                table: "account_info",
                column: "CheckAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_BRSTNCode",
                table: "bank_branchs",
                column: "BRSTNCode");

            migrationBuilder.CreateIndex(
                name: "IX_bank_branchs_BankId",
                table: "bank_branchs",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_check_accounts_AccountNo",
                table: "check_accounts",
                column: "AccountNo");

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_CheckAccountId",
                table: "check_orders",
                column: "CheckAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_check_orders_OrderFileId",
                table: "check_orders",
                column: "OrderFileId");

            migrationBuilder.CreateIndex(
                name: "IX_check_types_BankId",
                table: "check_types",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_CheckTypeId",
                table: "form_checks",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_form_checks_FormTypeId",
                table: "form_checks",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_form_types_BankId",
                table: "form_types",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_order_files_BatchName",
                table: "order_files",
                column: "BatchName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_info");

            migrationBuilder.DropTable(
                name: "bank_branchs");

            migrationBuilder.DropTable(
                name: "check_orders");

            migrationBuilder.DropTable(
                name: "form_checks");

            migrationBuilder.DropTable(
                name: "order_file_configuration");

            migrationBuilder.DropTable(
                name: "seed");

            migrationBuilder.DropTable(
                name: "account_addresses");

            migrationBuilder.DropTable(
                name: "check_accounts");

            migrationBuilder.DropTable(
                name: "order_files");

            migrationBuilder.DropTable(
                name: "check_types");

            migrationBuilder.DropTable(
                name: "form_types");

            migrationBuilder.DropTable(
                name: "banks_info");
        }
    }
}
