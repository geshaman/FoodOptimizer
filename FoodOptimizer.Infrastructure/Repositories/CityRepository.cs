using FoodOptimizer.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace FoodOptimizer.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;

        public CityRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetCitiesAsync()
        {
            return await _context.Restaurants
                .Select(r => r.City)
                .Distinct()
                .ToListAsync();
        }
    }
}
