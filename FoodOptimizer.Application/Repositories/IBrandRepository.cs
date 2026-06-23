using FoodOptimizer.Application.DTOs;
using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.Repositories
{
    public interface IBrandRepository
    {
        public Task<List<RestaurantBrandDTO>> GetBrandsByCityAsync(string city);
    }
}
