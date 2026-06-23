using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Parser.Models;
namespace FoodOptimizer.Parser.Services
{
    public interface IMenuSeedService
    {
        Task SeedAsync(IMenuParser parser, RestaurantSeedInfo info);
    }
}
