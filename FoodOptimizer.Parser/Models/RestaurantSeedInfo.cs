using FoodOptimizer.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Parser.Models
{
    public class RestaurantSeedInfo
    {
        public string BrandName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
