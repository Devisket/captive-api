using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class TagModelBuilder
    {
        public static void BuildTagModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Tag>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TagName).IsRequired(true);

            entity.HasMany(x => x.CheckInventory)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId);

            entity.HasMany(x => x.Products)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId);

            entity.HasMany(x => x.FormChecks)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId);

            entity.HasMany(x => x.BankBranches)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId);

            entity.HasMany(x => x.TagMappings)
                .WithOne(x => x.Tag)
                .HasForeignKey(x => x.TagId);

            entity.ToTable("tag");
        }

        public static void BuildTagMappingModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TagMapping>();

            entity.HasKey(x => x.Id);

            entity.ToTable("tag_mapping");
        }
    }

    
}
