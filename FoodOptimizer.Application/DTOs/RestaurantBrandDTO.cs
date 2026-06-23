using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.DTOs
{
    public class RestaurantBrandDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<RestaurantLocationDTO> Locations { get; set; }
    }
}
