using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOptimizer.Infrastructure.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.ToTable("restaurants");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(r => r.City)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(r => r.Address)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(r => r.OpenTime)
                .IsRequired();
            builder.Property(r => r.CloseTime)
                .IsRequired();
            builder.HasMany(r => r.MenuItems)
                .WithOne(m => m.Restaurant)
                .HasForeignKey(m => m.RestaurantId);
        }
    }
}