using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Domain
{
    public class Restaurant
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public long Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string City { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        public TimeOnly OpenTime { get; set; }

        [Required]
        public TimeOnly CloseTime { get; set; }

        public bool IsOpen => TimeOnly.FromDateTime(DateTime.Now) >= OpenTime
                && TimeOnly.FromDateTime(DateTime.Now) <= CloseTime;

        public long BrandId { get; set; }
        public RestaurantBrand Brand { get; set; }
    }
}
