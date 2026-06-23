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

        public string TimeZone { get; set; } = "Europe/Moscow";
        public bool IsOpen
        {
            get
            {
                var tz = NodaTime.DateTimeZoneProviders.Tzdb[TimeZone];
                var now = NodaTime.SystemClock.Instance.GetCurrentInstant();
                var localTime = now.InZone(tz).TimeOfDay;
                var localTimeOnly = new TimeOnly(localTime.Hour, localTime.Minute, localTime.Second);
                return localTimeOnly >= OpenTime && localTimeOnly <= CloseTime;
            }
        }

        public long? BrandId { get; set; }
        public RestaurantBrand Brand { get; set; }
    }
}
