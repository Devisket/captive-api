﻿using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static  class CheckOrdersModelBuilder
    {
        public static void BuildCheckOrdersModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckOrders>();

            entity.Property(x=> x.AccountNo).IsRequired();

            entity.Property(x=> x.OrderFileId).IsRequired();

            entity.Property(x=> x.BRSTN).IsRequired();

            entity.Property(x=> x.DeliverTo).IsRequired(false);

            //Configuration for one to many relationship for OrderFiles table
            entity.HasOne(x => x.OrderFile)
                .WithMany(x => x.CheckOrders)
                .HasForeignKey(x => x.OrderFileId)
                .OnDelete(DeleteBehavior.NoAction);

            //Configuration for one to one relationship for CheckAccounts table
            entity.HasOne(x => x.CheckAccount)
                .WithMany(x => x.CheckOrders)
                .HasForeignKey(x => x.CheckAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.ToTable("check_orders");
        }
    }
}
