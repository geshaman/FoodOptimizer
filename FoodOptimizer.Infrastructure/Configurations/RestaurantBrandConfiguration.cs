using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FoodOptimizer.Infrastructure.Configurations
{
    public class RestaurantBrandConfiguration : IEntityTypeConfiguration<RestaurantBrand>
    {
        public void Configure(EntityTypeBuilder<RestaurantBrand> builder)
        {
            builder.ToTable("restaurant_brands");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
            builder.HasMany(b => b.Locations)
                .WithOne(r => r.Brand)
                .HasForeignKey(r => r.BrandId);
        }
    }
}
