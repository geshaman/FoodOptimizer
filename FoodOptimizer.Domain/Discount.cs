using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Domain
{
    public class Discount
    {

        public long Id { get; set; }

        public long MenuItemId { get; set; }

        [Required]
        public MenuItem MenuItem { get; set; }

        public TimeOnly Start { get; set; }

        public TimeOnly End { get; set; }

        public bool IsAvailable => TimeOnly.FromDateTime(DateTime.Now) >= Start
            && TimeOnly.FromDateTime(DateTime.Now) <= End;

        public DiscountType DiscountType { get; set; }

        public decimal Value { get; set; }

        public ActivationType ActivationType { get; set; }
    }
}
