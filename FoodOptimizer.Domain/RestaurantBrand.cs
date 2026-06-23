using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Domain
{
    public class RestaurantBrand
    {

        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public List<Restaurant> Locations { get; set; }
    }
}
