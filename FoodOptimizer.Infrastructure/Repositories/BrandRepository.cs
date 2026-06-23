using FoodOptimizer.Application.Repositories;
using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Application.DTOs;
namespace FoodOptimizer.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RestaurantBrandDTO>> GetBrandsByCityAsync(string city)
        {
            return await _context.Brands
                .Where(b => b.Locations.Any(r => r.City == city))
                .Select(b => new RestaurantBrandDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Locations = b.Locations.Select(r => new RestaurantLocationDTO
                    {
                        Id = r.Id,
                        Address = r.Address,
                        IsOpen = r.IsOpen
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
