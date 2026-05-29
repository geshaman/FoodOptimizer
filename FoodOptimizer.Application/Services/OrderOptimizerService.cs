using FoodOptimizer.Application.DTOs;
using FoodOptimizer.Application.Repositories;
using FoodOptimizer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Application.Services
{
    public class OrderOptimizerService : IOrderOptimizerService
    {
        private readonly IRestaurantRepository _repository;

        public OrderOptimizerService(IRestaurantRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<OrderVariant>> GetOrderVariantsAsync(OptimizationRequest request)
        {
            Restaurant restaurant = await _repository.GetRestaurantWithMenuByIdAsync(request.RestaurantId)
                ?? throw new Exception($"Ресторан с ID {request.RestaurantId} не найден.");
            return request.Mode switch
            {
                OptimizationMode.MaxDiversity => BuildDiverseVariants(request, restaurant),
                OptimizationMode.MaxCalories => BuildCaloricVariants(request, restaurant),
                _ => throw new Exception("Неизвестный режим")
            };
        }

        private decimal GetEffectivePrice(MenuItem item)
        {
            var bestDiscount = GetBestDiscount(item);
            if (bestDiscount == null) return item.Price;

            return bestDiscount.DiscountType switch
            {
                DiscountType.FixedPrice => bestDiscount.Value,
                DiscountType.Percentage => item.Price * (1 - bestDiscount.Value / 100),
                _ => item.Price
            };
        }

        private Discount? GetBestDiscount(MenuItem item)
        {
            if (!item.Discounts.Any()) return null;

            return item.Discounts
                .Where(d => d.IsAvailable)
                .OrderBy(d => d.DiscountType == DiscountType.FixedPrice
                    ? d.Value
                    : item.Price * (1 - d.Value / 100))
                .FirstOrDefault();
        }

        private OrderItem BuildOrderItem(MenuItem menuItem)
        {
            OrderItem orderItem = new OrderItem();
            orderItem.ItemName = menuItem.Name;
            orderItem.ItemPrice = menuItem.Price;
            orderItem.EfficientItemPrice = GetEffectivePrice(menuItem);
            orderItem.Calories = menuItem.Calories;
            var bestDiscount = GetBestDiscount(menuItem);
            orderItem.DiscountType = bestDiscount?.DiscountType;
            string activationManual = bestDiscount?.ActivationType switch
            {
                ActivationType.QrCode => "QR-купону в приложении",
                ActivationType.Promocode => "промокоду",
                ActivationType.Automatic => "акции - скидка применится автоматически",
                null => "обычной цене",
                _ => "обычной цене"
            };
            orderItem.Manual = $"Позицию под названием {orderItem.ItemName} нужно " +
                $"взять по {activationManual} ({orderItem.EfficientItemPrice} руб)";
            return orderItem;
        }

        private List<OrderVariant> BuildDiverseVariants(OptimizationRequest request, Restaurant restaurant)
        {
            List<Category> desiredCategories = request.DesiredCategories;
            List<MenuItem> filteredMenu = restaurant.MenuItems.Where(m => desiredCategories.Contains(m.Category)).ToList();

            List<OrderVariant> result = new List<OrderVariant>();
            for (int i = 0; i < request.Count; i++)
            {
                OrderVariant variant = new OrderVariant();
                decimal remainingBudget = request.Budget;

                foreach (Category category in desiredCategories)
                {
                    var categoryItems = filteredMenu
                        .Where(m => m.Category == category
                            && GetEffectivePrice(m) <= remainingBudget)
                        .OrderBy(m => GetEffectivePrice(m))
                        .ToList();

                    int skip = category == Category.Entree ? i : Random.Shared.Next(0, categoryItems.Count);
                    MenuItem? bestItem = categoryItems.Skip(skip).FirstOrDefault()
                        ?? categoryItems.FirstOrDefault();

                    if (bestItem == null) continue;

                    variant.OrderItems.Add(BuildOrderItem(bestItem));
                    
                    remainingBudget -= GetEffectivePrice(bestItem);
                }
                variant.TotalPrice = variant.OrderItems.Sum(oi => oi.EfficientItemPrice);
                variant.OptimizationMode = request.Mode;
                variant.RestaurantName = restaurant.Name;
                result.Add(variant);
            }

            return result;
        }

        private List<OrderVariant> BuildCaloricVariants(OptimizationRequest request, Restaurant restaurant)
        {
            List<Category> desiredCategories = request.DesiredCategories;
            List<MenuItem> filteredMenu = restaurant.MenuItems.Where(m => desiredCategories.Contains(m.Category)).ToList();

            List<OrderVariant> result = new List<OrderVariant>();
            for (int i = 0; i < request.Count; i++)
            {
                OrderVariant variant = new OrderVariant();
                decimal remainingBudget = request.Budget;

                foreach (Category category in desiredCategories)
                {
                    int skip = category == Category.Entree ? i : Random.Shared.Next(0, 3);
                    MenuItem? bestItem = filteredMenu
                        .Where(m => m.Category == category
                            && GetEffectivePrice(m) <= remainingBudget)
                        .OrderByDescending(m => m.Calories / GetEffectivePrice(m))
                        .Skip(skip)
                        .FirstOrDefault();

                    if (bestItem == null) continue;

                    variant.OrderItems.Add(BuildOrderItem(bestItem));
                   
                    remainingBudget -= GetEffectivePrice(bestItem);
                }
                variant.TotalPrice = variant.OrderItems.Sum(oi => oi.EfficientItemPrice);
                variant.OptimizationMode = request.Mode;
                variant.RestaurantName = restaurant.Name;
                result.Add(variant);
            }

            return result;
        }
    }

}
