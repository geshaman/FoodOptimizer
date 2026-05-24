using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOptimizer.Infrastructure.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("discounts");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Start);
            builder.Property(d => d.End);
            builder.Property(d => d.DiscountType);
            builder.Property(d => d.ActivationType);
            builder.Property(d => d.Value)
                .HasColumnType("numeric(10,2)");
            builder.HasIndex(d => d.MenuItemId);
            builder.HasOne(d => d.MenuItem)
                .WithMany(m => m.Discounts)
                .HasForeignKey(d => d.MenuItemId);
        }
    }
}