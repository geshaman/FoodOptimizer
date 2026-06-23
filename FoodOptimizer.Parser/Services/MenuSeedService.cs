using FoodOptimizer.Domain;
using FoodOptimizer.Infrastructure;
using FoodOptimizer.Parser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Parser.Services
{
    public class MenuSeedService : IMenuSeedService
    {
        private readonly AppDbContext _context;

        public MenuSeedService(AppDbContext context)
        {
            _context = context;
        }
        public async Task SeedAsync(IMenuParser parser, RestaurantSeedInfo info)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Name == info.BrandName);
            if (brand == null)
            {
                brand = new RestaurantBrand { Name = info.BrandName };
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
            }

            var items = await parser.ParseMenuAsync();
            var restaurant = await _context
                .Restaurants
                .FirstOrDefaultAsync(r => r.Name == info.BrandName);

            if (restaurant != null && restaurant.BrandId == 0)
            {
                restaurant.BrandId = brand.Id;
                await _context.SaveChangesAsync();
            }

            if (restaurant == null)
            {
                restaurant = new Restaurant
                {
                    Address = info.Address,
                    Name = info.BrandName,
                    City = info.City,
                    OpenTime = info.OpenTime,
                    CloseTime = info.CloseTime,
                    Brand = brand
                };
                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();
            }

            _context.MenuItems.RemoveRange(
                _context.MenuItems.Where(m => m.RestaurantId == restaurant.Id));
            foreach (var parsed in items)
            {
                var category = parser.MapCategory(parsed.CategorySlug);
                if (category == null) continue;
                _context.MenuItems.Add(new MenuItem
                {
                    RestaurantId = restaurant.Id,
                    Name = parsed.Name,
                    Price = parsed.Price,
                    Calories = parsed.Calories,
                    Category = category.Value,
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
