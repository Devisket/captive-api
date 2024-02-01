﻿// <auto-generated />
using System;
using Captive.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Captive.Commands.Migrations
{
    [DbContext(typeof(CaptiveDataContext))]
    [Migration("20240201000605_AddFormCheckTable")]
    partial class AddFormCheckTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Captive.Data.Models.AccountAddresses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address1")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Address2")
                        .HasColumnType("longtext");

                    b.Property<string>("Address3")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("account_addresses", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.AccountInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AccountAddressId")
                        .HasColumnType("int");

                    b.Property<string>("Address1")
                        .HasColumnType("longtext");

                    b.Property<string>("Address2")
                        .HasColumnType("longtext");

                    b.Property<int>("CheckAccountId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AccountAddressId")
                        .IsUnique();

                    b.HasIndex("CheckAccountId")
                        .IsUnique();

                    b.ToTable("account_info", (string)null);
                });

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

                    b.Property<string>("BranchAddress")
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

                    b.HasKey("Id");

                    b.ToTable("banks_info", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckAccounts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("AccountNo");

                    b.ToTable("check_accounts", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("BRSTN")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("CheckAccountId")
                        .HasColumnType("int");

                    b.Property<string>("DeliverTo")
                        .HasColumnType("longtext");

                    b.Property<int>("OrderFileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CheckAccountId");

                    b.HasIndex("OrderFileId");

                    b.ToTable("check_orders", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.CheckTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("check_types", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CheckTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("FormTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CheckTypeId");

                    b.HasIndex("FormTypeId");

                    b.ToTable("form_checks", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.FormTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("FormType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("form_types", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFileConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConfigurationData")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("order_file_configuration", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFiles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BatchName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("ProcessDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BatchName");

                    b.ToTable("order_files", (string)null);
                });

            modelBuilder.Entity("Captive.Data.Models.AccountInfo", b =>
                {
                    b.HasOne("Captive.Data.Models.AccountAddresses", "AccountAddress")
                        .WithOne("AccountInfo")
                        .HasForeignKey("Captive.Data.Models.AccountInfo", "AccountAddressId");

                    b.HasOne("Captive.Data.Models.CheckAccounts", "CheckAccount")
                        .WithOne("AccountInfo")
                        .HasForeignKey("Captive.Data.Models.AccountInfo", "CheckAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountAddress");

                    b.Navigation("CheckAccount");
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

            modelBuilder.Entity("Captive.Data.Models.CheckOrders", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckAccounts", "CheckAccount")
                        .WithMany("CheckOrders")
                        .HasForeignKey("CheckAccountId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.OrderFiles", "OrderFile")
                        .WithMany("CheckOrders")
                        .HasForeignKey("OrderFileId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CheckAccount");

                    b.Navigation("OrderFile");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckTypes", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "Bank")
                        .WithMany("CheckTypes")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");
                });

            modelBuilder.Entity("Captive.Data.Models.FormChecks", b =>
                {
                    b.HasOne("Captive.Data.Models.CheckTypes", "CheckType")
                        .WithMany("FormChecks")
                        .HasForeignKey("CheckTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Captive.Data.Models.FormTypes", "FormType")
                        .WithMany("FormChecks")
                        .HasForeignKey("FormTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckType");

                    b.Navigation("FormType");
                });

            modelBuilder.Entity("Captive.Data.Models.FormTypes", b =>
                {
                    b.HasOne("Captive.Data.Models.BankInfo", "Bank")
                        .WithMany("FormTypes")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");
                });

            modelBuilder.Entity("Captive.Data.Models.AccountAddresses", b =>
                {
                    b.Navigation("AccountInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("Captive.Data.Models.BankInfo", b =>
                {
                    b.Navigation("BankBranches");

                    b.Navigation("CheckTypes");

                    b.Navigation("FormTypes");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckAccounts", b =>
                {
                    b.Navigation("AccountInfo")
                        .IsRequired();

                    b.Navigation("CheckOrders");
                });

            modelBuilder.Entity("Captive.Data.Models.CheckTypes", b =>
                {
                    b.Navigation("FormChecks");
                });

            modelBuilder.Entity("Captive.Data.Models.FormTypes", b =>
                {
                    b.Navigation("FormChecks");
                });

            modelBuilder.Entity("Captive.Data.Models.OrderFiles", b =>
                {
                    b.Navigation("CheckOrders");
                });
#pragma warning restore 612, 618
        }
    }
}
