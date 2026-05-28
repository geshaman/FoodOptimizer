using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.Repositories
{
    public interface IRestaurantRepository
    {
        public Task<List<Restaurant>> GetRestaurantsByCityAsync(string city);
        public Task<Restaurant?> GetRestaurantWithMenuByIdAsync(long id);
    }
}
