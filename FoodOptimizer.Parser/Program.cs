using FoodOptimizer.Domain;
using FoodOptimizer.Infrastructure;
using FoodOptimizer.Parser.Models;
using FoodOptimizer.Parser.Services;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var connectionString = "Host=localhost;Port=5433;Database=food_optimizer;Username=postgres;Password=65621598";

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new AppDbContext(optionsBuilder.Options);

var seedService = new MenuSeedService(context);

await seedService.SeedAsync(new RosticsMenuParser("74015171"), new RestaurantSeedInfo
{
    BrandName = "Rostics",
    City = "Пенза",
    Address = "ул. Московская, 78",
    OpenTime = new TimeOnly(8, 0),
    CloseTime = new TimeOnly(23, 0)
});

await seedService.SeedAsync(new VkusnoMenuParser(), new RestaurantSeedInfo
{
    BrandName = "Вкусно и Точка",
    City = "Пенза",
    Address = "ул. Московская, 78",
    OpenTime = new TimeOnly(8, 0),
    CloseTime = new TimeOnly(23, 0)
});

await seedService.SeedAsync(new BurgerKingMenuParser(), new RestaurantSeedInfo
{
    BrandName = "BURGER KING",
    City = "Пенза",
    Address = "ул. Московская, 85А",
    OpenTime = new TimeOnly(8, 0),
    CloseTime = new TimeOnly(23, 0)
});