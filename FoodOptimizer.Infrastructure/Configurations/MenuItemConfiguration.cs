using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOptimizer.Infrastructure.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("menu_items");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("numeric(10,2)");
            builder.Property(m => m.Category)
                .IsRequired();
            builder.Property(m => m.Calories)
                .IsRequired();
            builder.HasIndex(m => m.RestaurantId);
            builder.HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId);
        }
    }
}