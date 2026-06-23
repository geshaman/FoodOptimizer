using FoodOptimizer.Domain;
using FoodOptimizer.Parser.Helpers;
using FoodOptimizer.Parser.Models;
using FoodOptimizer.Parser.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public class BurgerKingMenuParser : IMenuParser
{
    private const string ApiUrl = "https://orderapp.burgerkingrus.ru/gateway/menu-composition/api/v7/menu";
    private readonly HttpClient _httpClient;
    public BurgerKingMenuParser()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://burgerkingrus.ru");
        _httpClient.DefaultRequestHeaders.Add("x-burgerking-platform", "web_mobile");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<ParsedMenuItem>> ParseMenuAsync()
    {
        var body = new
        {
            @params = new
            {
                restaurant_id = (string?)null,
                type = "stay",
                source = "web",
                keys = (string?)null
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(ApiUrl, content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var root = JsonNode.Parse(responseJson);
        var pricesKey = root?["result"]?["keys"]?["prices_key"]?.GetValue<string>() ?? "";
        Console.WriteLine($"prices_key: {pricesKey}");
        var groupsArray = root?["result"]?["groups"]?.AsArray();
        var resultNode = root?["result"];
        var firstGroup = root?["result"]?["categories"]?[0]?["menu_positions"]?[0];

        // Строим словарь id -> dish из result.dishes
        var dishesArray = root?["result"]?["dishes"]?.AsArray();
        if (dishesArray == null)
        {
            Console.WriteLine("dishes не найдены");
            return new List<ParsedMenuItem>();
        }

        var dishMap = new Dictionary<int, JsonNode>();
        foreach (var dish in dishesArray)
        {
            var mainInfo = dish?["main_info"];
            if (mainInfo == null) continue;
            var id = mainInfo["id"]?.GetValue<int>() ?? 0;
            if (id != 0) dishMap[id] = dish!;
        }

        var firstDish = dishMap.Values.FirstOrDefault();
        var groupMap = new Dictionary<int, JsonNode>();
        if (groupsArray != null)
        {
            foreach (var group in groupsArray)
            {
                var id = group?["main_info"]?["id"]?.GetValue<int>() ?? 0;
                if (id != 0) groupMap[id] = group!;
            }
        }
        var dishToGroupId = new Dictionary<int, int>();
        foreach (var (groupId, group) in groupMap)
        {
            var includedDishes = group["included_dishes"]?.AsArray();
            if (includedDishes == null) continue;
            foreach (var dishIdNode in includedDishes)
            {
                var dishId = dishIdNode?.GetValue<int>() ?? 0;
                if (dishId != 0) dishToGroupId[dishId] = groupId;
            }
        }

        var categories = root?["result"]?["categories"]?.AsArray();
        if (categories == null)
        {
            Console.WriteLine("Категории не найдены");
            return new List<ParsedMenuItem>();
        }

        var result = new List<ParsedMenuItem>();
        var seen = new HashSet<int>();

        foreach (var category in categories)
        {
            var categoryName = category?["name"]?.GetValue<string>() ?? "";
            var positions = category?["menu_positions"]?.AsArray();
            var subcategories = category?["subcategories"]?.AsArray();

            if (positions != null && positions.Count > 0)
            {
                await CollectDishes(positions, categoryName, dishMap, groupMap, seen, result, pricesKey, dishToGroupId);
            }
            else if (subcategories != null)
            {
                foreach (var sub in subcategories)
                {
                    var subPositions = sub?["menu_positions"]?.AsArray();
                    if (subPositions == null) continue;
                    await CollectDishes(subPositions, categoryName, dishMap, groupMap, seen, result, pricesKey, dishToGroupId);
                }
            }
        }

        Console.WriteLine($"Найдено товаров: {result.Count}");
        foreach (var item in result.Take(5))
            Console.WriteLine($"  {item.Name} — {item.Price}₽ [{item.CategorySlug}]");

        return result;
    }
    private async Task CollectDishes(JsonArray positions,
                               string categoryName,
                               Dictionary<int, JsonNode> dishMap,
                               Dictionary<int, JsonNode> groupMap,
                               HashSet<int> seen,
                               List<ParsedMenuItem> result,
                               string pricesKey,
                               Dictionary<int, int> dishToGroupId)
    {
        foreach (var position in positions)
        {
            var type = position?["type"]?.GetValue<string>();
            var id = position?["id"]?.GetValue<int>() ?? 0;

            if (type == "dish")
            {
                if (!seen.Add(id)) continue;
                if (!dishMap.TryGetValue(id, out var dish)) continue;

                var mainInfo = dish["main_info"]!;
                var restricted = mainInfo["restricted"]?.GetValue<bool>() ?? false;
                if (restricted) continue;

                var name = mainInfo["name"]?.GetValue<string>() ?? "";
                var priceRaw = mainInfo["price"]?.GetValue<int>() ?? 0;
                var price = priceRaw / 100m;
                if (string.IsNullOrEmpty(name) || price == 0) continue;

                var imageNode = mainInfo["image"]?["name"]?.GetValue<string>() ?? "";
                var imageUrl = string.IsNullOrEmpty(imageNode) ? ""
                    : $"https://images.burgerkingrus.ru/image/upload/w_460,h_460,c_fit,q_auto/{imageNode}";

                int kcals = 0;
                if (dishToGroupId.TryGetValue(id, out var groupId))
                {
                    var calories = await FetchGroupCaloriesAsync(groupId, pricesKey);
                    await Task.Delay(100);
                    calories.TryGetValue(id, out kcals);
                }
                else
                {
                    kcals = await FetchDishCaloriesAsync(id, pricesKey);
                    await Task.Delay(100);
                }

                result.Add(new ParsedMenuItem
                {
                    Name = name,
                    Price = price,
                    CategorySlug = categoryName,
                    PageUrl = "",
                    ImageUrl = imageUrl,
                    Calories = kcals
                });
            }
            else if (type == "group")
            {
                if (!groupMap.TryGetValue(id, out var group)) continue;
                var includedDishes = group["included_dishes"]?.AsArray();
                if (includedDishes == null) continue;

                var caloriesByDishId = await FetchGroupCaloriesAsync(id, pricesKey);
                await Task.Delay(100);
                foreach (var dishIdNode in includedDishes)
                {
                    var dishId = dishIdNode?.GetValue<int>() ?? 0;
                    if (!seen.Add(dishId)) continue; 
                    if (!dishMap.TryGetValue(dishId, out var dish)) continue;

                    var mainInfo = dish["main_info"]!;
                    var restricted = mainInfo["restricted"]?.GetValue<bool>() ?? false;
                    if (restricted) continue;

                    var name = mainInfo["name"]?.GetValue<string>() ?? "";
                    var priceRaw = mainInfo["price"]?.GetValue<int>() ?? 0;
                    var price = priceRaw / 100m;
                    if (string.IsNullOrEmpty(name) || price == 0) { Console.WriteLine($"  {dishId} — пустое имя или цена 0 (price={price}, name={name})"); continue; }

                    var imageNode = mainInfo["image"]?["name"]?.GetValue<string>() ?? "";
                    var imageUrl = string.IsNullOrEmpty(imageNode) ? ""
                        : $"https://images.burgerkingrus.ru/image/upload/w_460,h_460,c_fit,q_auto/{imageNode}";

                    var calories = caloriesByDishId.TryGetValue(dishId, out var cal) ? cal : 0;
                    result.Add(new ParsedMenuItem
                    {
                        Name = name,
                        Price = price,
                        CategorySlug = categoryName,
                        PageUrl = "",
                        ImageUrl = imageUrl,
                        Calories = calories
                    });
                }
            }
        }
    }
    private async Task<Dictionary<int, int>> FetchGroupCaloriesAsync(int groupId, string pricesKey)
    {
        var body = new
        {
            @params = new
            {
                id = groupId,
                restaurant_id = (string?)null,
                type = "stay",
                source = "web",
                keys = new {prices_key = pricesKey}
            }
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(
                "https://orderapp.burgerkingrus.ru/gateway/menu-composition/api/v7/menu/group",
                content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var root = JsonNode.Parse(responseJson);

            var dishes = root?["result"]?["group"]?["dishes"]?.AsArray();
            var caloriesByDishId = new Dictionary<int, int>();

            foreach (var dish in dishes ?? new JsonArray())
            {
                var dishId = dish?["main_info"]?["id"]?.GetValue<int>() ?? 0;
                var kcals = dish?["composition"]?["kcals"]?["amount"]?.GetValue<int>() ?? 0;
                if (dishId != 0) caloriesByDishId[dishId] = kcals;
            }

            return caloriesByDishId;
        }
        catch
        {
            return new Dictionary<int, int>();
        }
    }
    private async Task<int> FetchDishCaloriesAsync(int dishId, string pricesKey)
    {
        var body = new
        {
            @params = new
            {
                id = dishId,
                restaurant_id = (string?)null,
                type = "stay",
                source = "web",
                keys = new { prices_key = pricesKey }
            }
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(
                "https://orderapp.burgerkingrus.ru/gateway/menu-composition/api/v7/menu/dish",
                content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var root = JsonNode.Parse(responseJson);

            // Проверь реальный путь после первого запуска
            return root?["result"]?["dish"]?["composition"]?["kcals"]?["amount"]?.GetValue<int>() ?? 0;
        }
        catch
        {
            return 0;
        }
    }
    public Category? MapCategory(string slug) =>
            CategoryMapper.GetCategory(slug);
}