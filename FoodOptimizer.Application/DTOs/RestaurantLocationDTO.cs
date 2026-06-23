using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.DTOs
{
    public class RestaurantLocationDTO
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public bool IsOpen { get; set; }

    }
}
