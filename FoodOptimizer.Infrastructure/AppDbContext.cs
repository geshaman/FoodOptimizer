using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Domain;
using FoodOptimizer.Infrastructure.Configurations;

namespace FoodOptimizer.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<RestaurantBrand> Brands { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RestaurantConfiguration());
            modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountConfiguration());
            modelBuilder.ApplyConfiguration(new RestaurantBrandConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
