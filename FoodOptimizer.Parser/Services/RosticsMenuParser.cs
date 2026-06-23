using FoodOptimizer.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FoodOptimizer.Parser.Helpers;
using FoodOptimizer.Domain;
namespace FoodOptimizer.Parser.Services
{
    public class RosticsMenuParser : IMenuParser
    {
        private const string BaseUrl = "https://rostics.ru/api/menu/getmenu";
        private const string PdfUrl = "https://s82079.cdn.ngenix.net/v2qqjxcbqdte2du2jm0l56s97lgh.pdf";
        
        private readonly HttpClient _httpClient;
        private readonly string _storeId;
        private Dictionary<string, int>? _pdfCalories;

        public RosticsMenuParser(string storeId)
        {
            _storeId = storeId;
            _httpClient = new HttpClient();
        }

        public async Task<List<ParsedMenuItem>> ParseMenuAsync()
        {
            _pdfCalories = await LoadPdfCaloriesAsync();

            var result = new List<ParsedMenuItem>();
            var url = $"{BaseUrl}/{_storeId}/ru/clickcollect?requestId={Guid.NewGuid()}";

            var response = await _httpClient.GetAsync(url);
            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Статус: {response.StatusCode}");
            var root = JsonNode.Parse(responseJson);
            var products = root?["value"]?["products"]?.AsObject();

            if (products == null)
            {
                Console.WriteLine("products не найдены");
                return new List<ParsedMenuItem>();
            }

            foreach (var product in products)
            {
                var data = product.Value;
                var name = data?["title"]?.GetValue<string>() ?? "";
                var priceRaw = data?["price"]?.GetValue<int>() ?? 0;
                if (string.IsNullOrEmpty(name) || priceRaw == 0) continue;
                if (name.Contains("Купон", StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Contains("Салфетка", StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Contains("Игрушка", StringComparison.OrdinalIgnoreCase)) continue;
                var price = priceRaw / 100m;
                var image = data?["image"]?.GetValue<string>() ?? "";
                var imageUrl = "https://s82079.cdn.ngenix.net/330x0/" + image;
                var per100 = (int) (data?["energy"]?["ccal"]?.GetValue<double>() ?? 0);
                var volume = data?["volume"]?["gr"]?.GetValue<double>()
                    ?? data?["volume"]?["ml"]?.GetValue<double>()
                    ?? 0;
                var calories = (int) Math.Round(per100 * volume / 100);
                if (calories == 0)
                    calories = LookupPdfCalories(name);
                var category = CategoryMapper.GetCategoryByTitle(name);
                if (category == null)
                {
                    Console.WriteLine($"Не удалось определить категорию {name}");
                    continue;
                }
                result.Add(new ParsedMenuItem
                {
                    Name = name,
                    Price = price,
                    CategorySlug = category.ToString(),
                    Calories = calories,
                    ImageUrl = imageUrl,
                    PageUrl = ""
                });
            }
            return result;
        }

        private static readonly Dictionary<string, int> SauceFallback =
            new(StringComparer.OrdinalIgnoreCase)
        {
            ["соус кисло-сладкий чили"] = 140,
            ["соус барбекю"] = 120,
            ["соус терияки"] = 130,
            ["соус чесночный"] = 150,
            ["кетчуп томатный"] = 90,
            ["соус васаби"] = 100,
            ["шеф баскет с наггетсами"] = 731,
            ["мой баскет с оригинальными ножками"] = 735,
            ["мой баскет с крылышками"] = 656,
        };

        private int LookupPdfCalories(string apiName)
        {
            if (_pdfCalories == null) return 0;

            var normalizedApi = RosticsPdfCaloriesParser.NormalizeName(apiName);

            if (_pdfCalories.TryGetValue(normalizedApi, out var cal))
                return cal;

            foreach (var (pdfKey, mappedApi) in RosticsPdfNameMapper.PdfToApi)
            {
                if (string.Equals(mappedApi, normalizedApi, StringComparison.OrdinalIgnoreCase)
                    && _pdfCalories.TryGetValue(pdfKey, out var mappedCal))
                    return mappedCal;
            }

            if (SauceFallback.TryGetValue(normalizedApi, out var sauceCal))
                return sauceCal;
            return 0;
        }

        private async Task<Dictionary<string, int>> LoadPdfCaloriesAsync()
        {
            try
            {
                var parser = new RosticsPdfCaloriesParser();
                return await parser.ParseFromUrlAsync(PdfUrl);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, int>();
            }
        }

        public Category? MapCategory(string slug) =>
            Enum.TryParse<Category>(slug, out var cat) ? cat : null;
    }
}
