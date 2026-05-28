using FoodOptimizer.Application.Repositories;
using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Infrastructure.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly AppDbContext _context;

        public RestaurantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Restaurant>> GetRestaurantsByCityAsync(string city)
        {
            return await _context.Restaurants
                .Where(r => r.City == city)
                .ToListAsync();
        }

        public async Task<Restaurant?> GetRestaurantWithMenuByIdAsync(long id)
        {
            return await _context.Restaurants
                .Include(r => r.MenuItems)
                .ThenInclude(m => m.Discounts)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
