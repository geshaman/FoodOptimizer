using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.DTOs
{
    public class OrderVariant
    {
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal TotalPrice { get; set; }

        public OptimizationMode OptimizationMode { get; set; }

        public string RestaurantName { get; set; }
    }
}
