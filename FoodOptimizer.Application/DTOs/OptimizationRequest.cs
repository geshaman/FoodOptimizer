using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Domain;

namespace FoodOptimizer.Application.DTOs
{
    public class OptimizationRequest
    {
        public long RestaurantId { get; set; }

        [Range(1, 100000)]
        public decimal Budget { get; set; }

        public OptimizationMode Mode { get; set; }

        [MinLength(1)]
        public List<Category> DesiredCategories { get; set; }

        [Range(1, 10)]
        public int Count { get; set; } = 3;
    }
}
