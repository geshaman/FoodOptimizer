using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FoodOptimizer.Domain;

public class MenuItem
{
    public long Id { get; set; }

    public long RestaurantId { get; set; }

    public List<Discount> Discounts { get; set; } = new List<Discount>();

    [Required]
    public Restaurant Restaurant { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [Required]
    public Category Category { get; set; }

    [Required]
    public int Calories { get; set; }
}
