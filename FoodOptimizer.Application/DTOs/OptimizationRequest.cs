using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Domain;

namespace FoodOptimizer.Application.DTOs
{
    public class OptimizationRequest
    {
        public long RestaurantId { get; set; }

        public decimal Budget { get; set; }

        public OptimizationMode Mode { get; set; }

        public List<Category> DesiredCategories { get; set; }
    }
}
