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

        public async Task<Restaurant?> GetRestaurantWithMenuByIdAsync(long id)
        {
            return await _context.Restaurants
                .Include(r => r.MenuItems)
                .ThenInclude(m => m.Discounts)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
