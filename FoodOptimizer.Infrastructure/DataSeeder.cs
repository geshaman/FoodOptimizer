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
                        Name = "Чикен Премьер",
                        Price = 239,
                        Category = Category.Entree,
                        Calories = 525,
                        Discounts = new List<Discount>{
                            new Discount
                            {
                                DiscountType = DiscountType.FixedPrice,
                                Value = 50,
                                ActivationType = ActivationType.Automatic,
                                Start = new TimeOnly(9, 0),
                                End = new TimeOnly(23, 0)
                            }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Картофель Фри Средний",
                        Price = 117,
                        Category = Category.Garnish,
                        Calories = 318
                    },
                    new MenuItem
                    {
                        Name = "Наггетсы 6 шт.",
                        Price = 107,
                        Category = Category.Garnish,
                        Calories = 268,
                        Discounts = new List<Discount>{
                            new Discount
                            {
                                DiscountType = DiscountType.FixedPrice,
                                Value = 107-99,
                                ActivationType = ActivationType.Automatic,
                                Start = new TimeOnly(9, 0),
                                End = new TimeOnly(23, 0)
                            }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Добрый Кола Средний",
                        Price = 146,
                        Category = Category.Drinks,
                        Calories = 106
                    },
                    new MenuItem
                    {
                        Name = "Апельсиновый Сок Средний",
                        Price = 162,
                        Category = Category.Drinks,
                        Calories = 187
                    },
                    new MenuItem
                    {
                        Name = "Липтон Зелёный Средний",
                        Price = 146,
                        Category = Category.Drinks,
                        Calories = 106
                    },
                    new MenuItem
                    {
                        Name = "Айс Де Люкс Карамель",
                        Price = 147,
                        Category = Category.Dessert,
                        Calories = 374
                    },
                    new MenuItem
                    {
                        Name = "Соус Сырный",
                        Price = 48,
                        Category = Category.Sauces,
                        Calories = 91
                    }
                }
            };

            context.Restaurants.Add(restaurant);
            context.SaveChanges();
        }
    }
}
