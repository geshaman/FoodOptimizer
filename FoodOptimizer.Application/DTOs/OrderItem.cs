using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.DTOs
{
    public class OrderItem
    {
        public string ItemName { get; set; }

        public decimal ItemPrice { get; set; }

        public int Calories { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal EfficientItemPrice { get; set; }

        public string Manual { get; set; }
    }
}
