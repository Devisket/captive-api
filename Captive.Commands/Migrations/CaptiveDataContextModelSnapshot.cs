﻿// <auto-generated />
using System;
using Captive.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Captive.Data.Models.BankBranches", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BRSTNCode")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("BranchAddress1")
                        .HasColumnType("longtext");

                    b.Property<string>("BranchAddress2")
                        .HasColumnType("longtext");

                    b.Property<string>("BranchAddress3")
                        .HasColumnType("longtext");

                    b.Property<string>("BranchAddress4")
                        .HasColumnType("longtext");

                    b.Property<string>("BranchAddress5")
                        .HasColumnType("longtext");

                    b.Property<string>("BranchName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BRSTNCode");

                    b.HasIndex("BankId");

                    b.ToTable("bank_branchs", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BankInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BankDescription")
                        .HasColumnType("longtext");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("banks_info", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BatchFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankInfoId")
                        .HasColumnType("int");

                    b.Property<int>("BatchFileStatus")
                        .HasColumnType("int");

                    b.Property<string>("BatchName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BankInfoId");

                    b.ToTable("batch_files", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int?>("CheckOrderId")
                        .HasColumnType("int");

                    b.Property<string>("EndSeries")
                        .HasColumnType("longtext");

                    b.Property<int>("FormCheckId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("StarSeries")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("FormCheckId");

                    b.ToTable("check_inventory", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("BRSTN")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Concode")
                        .HasColumnType("longtext");

                    b.Property<string>("DeliverTo")
                        .HasColumnType("longtext");

                    b.Property<int>("FormCheckId")
                        .HasColumnType("int");

                    b.Property<int>("OrderFileId")
                        .HasColumnType("int");

                    b.Property<int>("OrderQuanity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FormCheckId");

                    b.HasIndex("OrderFileId");

                    b.ToTable("check_orders", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("CheckType")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("FileInitial")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FormType")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("ProductTypeId");

                    b.HasIndex("FormType", "CheckType");

                    b.ToTable("form_checks", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BatchFileId")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("ProcessDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BatchFileId");

                    b.HasIndex("FileName");

                    b.ToTable("order_files", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("ConfigurationData")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ConfigurationType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("OtherFileName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("Name");

                    b.ToTable("order_file_configuration", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LogType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("OrderFileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderFileId");

                    b.ToTable("order_file_logs", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.ProductConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("OrderFileConfigurationId")
                        .HasColumnType("int");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderFileConfigurationId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("product_configuration", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.ProductType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankInfoId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BankInfoId");

                    b.ToTable("product_type", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.Seeds", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("SeedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SeedName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("seed", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.BankBranches", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany("BankBranches")
                        .HasForeignKey("BankId")
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
                    b.HasOne("Captive.Data.Models.BankBranches", "BankBranch")
                        .WithMany("CheckInventory")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.FormChecks", "FormChecks")
                        .WithMany("CheckInventory")
                        .HasForeignKey("FormCheckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankBranch");

                    b.Navigation("FormChecks");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.HasOne("Captive.Data.Models.FormChecks", "FormChecks")
                        .WithMany("CheckOrders")
                        .HasForeignKey("FormCheckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.OrderFile", "OrderFile")
                        .WithMany("CheckOrders")
                        .HasForeignKey("OrderFileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FormChecks");

                    b.Navigation("OrderFile");
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "Bank")
                        .WithMany("FormChecks")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.ProductType", "ProductType")
                        .WithMany("FormChecks")
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.HasOne("Captive.Data.Models.BatchFile", "BatchFile")
                        .WithMany("OrderFiles")
                        .HasForeignKey("BatchFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BatchFile");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileConfiguration", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "Bank")
                        .WithMany("OrderFileConfigurations")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");
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

            modelBuilder.Entity("Captive.Data.Models.ProductConfiguration", b =>
                {
                    b.HasOne("Captive.Data.Models.OrderFileConfiguration", "OrderFileConfiguration")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("OrderFileConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.ProductType", "ProductType")
                        .WithMany("ProductConfiguration")
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderFileConfiguration");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("Captive.Data.Models.ProductType", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "BankInfo")
                        .WithMany("ProductTypes")
                        .HasForeignKey("BankInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankInfo");
                });

            modelBuilder.Entity("Captive.Data.Models.BankBranches", b =>
                {
                    b.Navigation("CheckInventory");
                });

            modelBuilder.Entity("Captive.Data.Models.BankInfo", b =>
                {
                    b.Navigation("BankBranches");

                    b.Navigation("BatchFiles");

                    b.Navigation("FormChecks");

                    b.Navigation("OrderFileConfigurations");

                    b.Navigation("ProductTypes");
                });

            modelBuilder.Entity("Captive.Data.Models.BatchFile", b =>
                {
                    b.Navigation("OrderFiles");
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.Navigation("CheckInventory");

                    b.Navigation("CheckOrders");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFile", b =>
                {
                    b.Navigation("CheckOrders");

                    b.Navigation("OrderFileLogs");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileConfiguration", b =>
                {
                    b.Navigation("ProductConfigurations");
                });

            modelBuilder.Entity("Captive.Data.Models.ProductType", b =>
                {
                    b.Navigation("FormChecks");

                    b.Navigation("ProductConfiguration");
                });
#pragma warning restore 612, 618
        }
    }
}
