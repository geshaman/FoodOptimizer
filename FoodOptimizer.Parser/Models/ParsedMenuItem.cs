using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Parser.Models
{
    public class ParsedMenuItem
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Calories { get; set; }

        public string CategorySlug { get; set; }

        public string ImageUrl { get; set; }

        public string PageUrl { get; set; }

        public int ExternalId { get; set; }
    }
}
