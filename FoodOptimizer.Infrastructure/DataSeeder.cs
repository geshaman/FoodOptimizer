using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Infrastructure
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Restaurants.Any()) return;

            var restaurant = new Restaurant
            {
                Name = "Вкусно и точка",
                City = "Пенза",
                Address = "ул. Московская, 73",
                OpenTime = new TimeOnly(9, 0),
                CloseTime = new TimeOnly(23, 0),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Name = "Биг Хит",
                        Price = 239,
                        Category = Category.Entree,
                        Calories = 524,
                        Discounts = new List<Discount>{
                            new Discount
                            {
                                DiscountType = DiscountType.Percentage,
                                Value = 20,
                                ActivationType = ActivationType.QrCode,
                                Start = new TimeOnly(9, 0),
                                End = new TimeOnly(23, 0)
                            }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Картофель фри Средний",
                        Price = 117,
                        Category = Category.Garnish,
                        Calories = 318
                    },
                    new MenuItem
                    {
                        Name = "Добрый Кола Средний",
                        Price = 146,
                        Category = Category.Drinks,
                        Calories = 106
                    }
                }
            };

            context.Restaurants.Add(restaurant);
            context.SaveChanges();
        }
    }
}
