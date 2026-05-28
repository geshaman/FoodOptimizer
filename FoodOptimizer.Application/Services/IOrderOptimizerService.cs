using FoodOptimizer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.Services
{
    public interface IOrderOptimizerService
    {
        public Task<List<OrderVariant>> GetOrderVariantsAsync(OptimizationRequest request);
    }
}
