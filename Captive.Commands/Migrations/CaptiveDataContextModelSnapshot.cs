﻿// <auto-generated />
using System;
using Captive.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Captive.Commands.Migrations
{
    [DbContext(typeof(CaptiveDataContext))]
    partial class CaptiveDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Captive.Data.Models.BankBranches", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BRSTNCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("BankInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BranchAddress1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchAddress2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchAddress3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchAddress4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchAddress5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MergingBranchId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BRSTNCode");

                    b.HasIndex("BankInfoId");

                    b.ToTable("bank_branchs", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BankInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BankDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("banks_info", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BatchFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BankInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BatchFileStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BatchName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankInfoId");

                    b.ToTable("batch_files", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckValidationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SeriesPatern")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CheckValidationId")
                        .IsUnique();

                    b.ToTable("check_inventory", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventoryDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("BranchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckInventoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CheckOrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EndingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FormCheckId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsReserve")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("StartingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CheckInventoryId");

                    b.HasIndex("CheckOrderId");

                    b.ToTable("check_inventory_detail", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BRSTN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("BranchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Concode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliverTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FormCheckId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OrderQuanity")
                        .HasColumnType("int");

                    b.Property<string>("PreEndingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreStartingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderFileId");

                    b.ToTable("check_orders", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckValidation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BankInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ValidateByBranch")
                        .HasColumnType("bit");

                    b.Property<bool>("ValidateByFormCheck")
                        .HasColumnType("bit");

                    b.Property<bool>("ValidateByProduct")
                        .HasColumnType("bit");

                    b.Property<string>("ValidationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BankInfoId");

                    b.ToTable("check_validation", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.FloatingCheckOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountName1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountName2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BRSTN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CheckType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Concode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliverTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<Guid>("OrderFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PreEndingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PreStartingSeries")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderFileId");

                    b.ToTable("floating_check_orders", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CheckType")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileInitial")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormType")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("FormType", "CheckType");

                    b.ToTable("form_checks", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BatchFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ProcessDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BatchFileId");

                    b.HasIndex("FileName");

                    b.HasIndex("ProductId");

                    b.ToTable("order_files", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrderFileId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrderFileId");

                    b.ToTable("order_file_logs", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BankInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BankInfoId");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.ProductConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckValidationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConfigurationData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConfigurationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CheckValidationId");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("product_configuration", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.Seeds", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("SeedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SeedName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("seed", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckValidationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CheckValidationId");

                    b.ToTable("tag", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.TagMapping", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BranchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FormCheckId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("tag_mapping", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BankBranches", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany("BankBranches")
                        .HasForeignKey("BankInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankInfo");
                });

            modelBuilder.Entity("Captive.Data.Models.BatchFile", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany("BatchFiles")
                        .HasForeignKey("BankInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankInfo");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventory", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckValidation", "CheckValidation")
                        .WithOne("CheckInventory")
                        .HasForeignKey("Captive.Data.Models.CheckInventory", "CheckValidationId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("CheckValidation");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventoryDetail", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckInventory", "CheckInventory")
                        .WithMany("CheckInventoryDetails")
                        .HasForeignKey("CheckInventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.CheckOrders", "CheckOrder")
                        .WithMany("CheckInventoryDetail")
                        .HasForeignKey("CheckOrderId");

                    b.Navigation("CheckInventory");

                    b.Navigation("CheckOrder");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.HasOne("Captive.Data.Models.OrderFile", "OrderFile")
                        .WithMany("CheckOrders")
                        .HasForeignKey("OrderFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderFile");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckValidation", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany()
                        .HasForeignKey("BankInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankInfo");
                });

            modelBuilder.Entity("Captive.Data.Models.FloatingCheckOrder", b =>
                {
                    b.HasOne("Captive.Data.Models.OrderFile", "OrderFile")
                        .WithMany("FloatingCheckOrders")
                        .HasForeignKey("OrderFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderFile");
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.HasOne("Captive.Data.Models.Product", "Product")
                        .WithMany("FormChecks")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.HasOne("Captive.Data.Models.BatchFile", "BatchFile")
                        .WithMany("OrderFiles")
                        .HasForeignKey("BatchFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.Product", "Product")
                        .WithMany("OrderFiles")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("BatchFile");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileLog", b =>
                {
                    b.HasOne("Captive.Data.Models.OrderFile", "OrderFile")
                        .WithMany("OrderFileLogs")
                        .HasForeignKey("OrderFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderFile");
                });

            modelBuilder.Entity("Captive.Data.Models.Product", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany("Products")
                        .HasForeignKey("BankInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankInfo");
                });

            modelBuilder.Entity("Captive.Data.Models.ProductConfiguration", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckValidation", "CheckValidation")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("CheckValidationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.Product", "Product")
                        .WithOne("ProductConfiguration")
                        .HasForeignKey("Captive.Data.Models.ProductConfiguration", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckValidation");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Captive.Data.Models.Tag", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckValidation", "CheckValidation")
                        .WithMany("Tags")
                        .HasForeignKey("CheckValidationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckValidation");
                });

            modelBuilder.Entity("Captive.Data.Models.TagMapping", b =>
                {
                    b.HasOne("Captive.Data.Models.Tag", "Tag")
                        .WithMany("Mapping")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Captive.Data.Models.BankInfo", b =>
                {
                    b.Navigation("BankBranches");

                    b.Navigation("BatchFiles");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("Captive.Data.Models.BatchFile", b =>
                {
                    b.Navigation("OrderFiles");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventory", b =>
                {
                    b.Navigation("CheckInventoryDetails");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.Navigation("CheckInventoryDetail");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckValidation", b =>
                {
                    b.Navigation("CheckInventory")
                        .IsRequired();

                    b.Navigation("ProductConfigurations");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.Navigation("CheckOrders");

                    b.Navigation("FloatingCheckOrders");

                    b.Navigation("OrderFileLogs");
                });

            modelBuilder.Entity("Captive.Data.Models.Product", b =>
                {
                    b.Navigation("FormChecks");

                    b.Navigation("OrderFiles");

                    b.Navigation("ProductConfiguration")
                        .IsRequired();
                });

            modelBuilder.Entity("Captive.Data.Models.Tag", b =>
                {
                    b.Navigation("Mapping");
                });
#pragma warning restore 612, 618
        }
    }
}
