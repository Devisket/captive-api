﻿using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class BankInfoModelBuilder
    {
        public static void BuildBankInfoModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<BankInfo>();

            entity.HasKey(x=> x.Id);

            entity.HasMany(x => x.BankBranches)
                .WithOne(x => x.BankInfo)
                .HasForeignKey(x => x.BankInfoId);

            entity.HasMany(x => x.BatchFiles)
                .WithOne(x => x.BankInfo)
                .HasForeignKey(x => x.BankInfoId);

            entity.HasMany(x => x.Products)
                .WithOne(x => x.BankInfo)
                .HasForeignKey(x => x.BankInfoId);

            entity.HasMany(x => x.Tags)
                .WithOne(x => x.BankInfo)
                .HasForeignKey(x => x.BankInfoId);

            entity.Property(x=> x.BankName).IsRequired();

            entity.Property(x=> x.CreatedDate).IsRequired();

            entity.Property(x => x.ShortName).IsRequired();

            entity.Property(x => x.BankDescription).IsRequired(false);

            entity.ToTable("banks_info");
        }
    }
}
