using FoodOptimizer.Domain;
using FoodOptimizer.Parser.Models;
using HtmlAgilityPack;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using FoodOptimizer.Parser.Helpers;

namespace FoodOptimizer.Parser.Services
{
    public class VkusnoMenuParser : IMenuParser
    {
        private const string MenuUrl = "https://vkusnoitochka.ru/menu";

        public async Task<List<ParsedMenuItem>> ParseMenuAsync()
        {
            var result = new List<ParsedMenuItem>();

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();

            Console.WriteLine("Открываем страницу меню...");
            await page.GotoAsync(MenuUrl, new()
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });

            await page.WaitForTimeoutAsync(2000);

            var html = await page.ContentAsync();
            Console.WriteLine($"Получили HTML, длина: {html.Length}");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var script = doc.DocumentNode
                .SelectSingleNode("//script[@id='__NUXT_DATA__']");

            if (script == null)
            {
                Console.WriteLine("__NUXT_DATA__ не найден");
                return result;
            }

            var items = ParseNuxtData(script.InnerText);
            Console.WriteLine($"Найдено товаров: {items.Count}");

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            Console.WriteLine("Загружаем калории (первый проход)...");
            await FetchCaloriesAsync(items, http);

            Console.WriteLine("Второй подход пошёл...");
            items = await ExpandItemsWithSizezAsync(items, http);
            Console.WriteLine($"Итого позиций после разворачивания: {items.Count}");

            return items;
        }
        public List<ParsedMenuItem> ParseNuxtData(string nuxtJson)
        {
            var result = new List<ParsedMenuItem>();
            var array = JsonNode.Parse(nuxtJson)!.AsArray();

            // Строим маппинг productId -> categorySlug
            var productToCategory = new Dictionary<int, string>();

            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] is not JsonObject obj) continue;
                if (!obj.ContainsKey("slug") || !obj.ContainsKey("products")) continue;

                // slug — это индекс на строку
                var categorySlug = Resolve<string>(array, obj["slug"]);
                if (string.IsNullOrEmpty(categorySlug)) continue;

                // products — это индекс на массив
                var productsRaw = obj["products"];
                JsonArray? productIds = null;

                if (productsRaw is JsonValue pv && pv.TryGetValue<int>(out var pi))
                    productIds = array[pi] as JsonArray;

                if (productIds == null) continue;

                foreach (var pidNode in productIds)
                {
                    if (pidNode is not JsonValue pval || !pval.TryGetValue<int>(out var arrayIndex))
                        continue;

                    // arrayIndex — это индекс в массиве, по которому лежит реальный ID товара
                    var realIdNode = array[arrayIndex];
                    if (realIdNode is JsonValue realIdVal && realIdVal.TryGetValue<int>(out var realId))
                        productToCategory.TryAdd(realId, categorySlug);
                }
            }

            // Ищем маппинг { "9020": 340, ... }
            JsonObject? productIndexMap = null;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] is not JsonObject obj) continue;
                var keys = obj.Select(k => k.Key).ToList();
                if (keys.Count > 5 && keys.All(k => int.TryParse(k, out _)))
                {
                    productIndexMap = obj;
                    break;
                }
            }

            if (productIndexMap == null)
            {
                Console.WriteLine("Маппинг товаров не найден");
                return result;
            }

            foreach (var (productId, indexNode) in productIndexMap)
            {
                var schemaIndex = indexNode!.GetValue<int>();
                if (array[schemaIndex] is not JsonObject schema) continue;
                if (!schema.ContainsKey("name") ||
                    !schema.ContainsKey("price") ||
                    !schema.ContainsKey("slug")) continue;

                try
                {
                    var name = Resolve<string>(array, schema["name"]);
                    var slug = Resolve<string>(array, schema["slug"]);
                    var price = Resolve<decimal>(array, schema["price"]);
                    var image = Resolve<string>(array, schema["image"]);

                    if (string.IsNullOrEmpty(name) || price == 0) continue;

                    // Берём категорию из маппинга, а не из слага товара
                    int.TryParse(productId, out var pid);
                    var categorySlug = productToCategory.TryGetValue(pid, out var cat)
                        ? cat : "unknown";

                    result.Add(new ParsedMenuItem
                    {
                        Name = name,
                        Price = price,
                        CategorySlug = categorySlug,
                        PageUrl = $"https://vkusnoitochka.ru/{slug}",
                        ImageUrl = string.IsNullOrEmpty(image) ? ""
                                       : $"https://vkusnoitochka.ru{image}",
                        Calories = 0,
                        ExternalId = pid
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка товара {productId}: {ex.Message}");
                }
            }
            return result;
        }

        private T? Resolve<T>(JsonArray array, JsonNode? node)
        {
            if (node is JsonValue val)
            {
                // Если это целое число — это индекс
                if (val.TryGetValue<int>(out var index))
                {
                    var target = array[index];
                    if (target is JsonValue tv)
                        return tv.GetValue<T>();
                    return default;
                }

                // Иначе это само значение
                return val.GetValue<T>();
            }
            return default;
        }

        public async Task FetchCaloriesAsync(List<ParsedMenuItem> items, HttpClient http)
        {
            foreach (var item in items)
            {
                try
                {
                    var url = $"https://vkusnoitochka.ru/api/nutrition/product/{item.ExternalId}";
                    var json = await http.GetStringAsync(url);
                    var doc = JsonNode.Parse(json);

                    var amountStr = doc?["nutritionalValue"]?["energyCal"]?["amount"]?.GetValue<string>();
                    if (int.TryParse(amountStr, out var cal))
                    {
                        item.Calories = cal;
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Калории не получены для {item.Name}: {ex.Message}");
                }
                await Task.Delay(200);
            }
        }

        public async Task<List<ParsedMenuItem>> ExpandItemsWithSizezAsync(
            List<ParsedMenuItem> items, HttpClient http)
        {
            var result = new List<ParsedMenuItem>();

            foreach (var item in items)
            {
                if (item.Calories > 0)
                {
                    result.Add(item);
                    continue;
                }

                try
                {
                    var url = $"https://vkusnoitochka.ru/api/menu/restaurant/25803/product/{item.ExternalId}";
                    var json = await http.GetStringAsync(url);
                    var doc = JsonNode.Parse(json);
                    var offers = doc?["offers"]?.AsArray();
                    if (offers == null || offers.Count == 0)
                    {
                        result.Add(item);
                        continue;
                    }

                    if (offers.Count == 1)
                    {
                        var offer = offers[0]!;
                        item.Price = offer?["price"]?.GetValue<decimal>() ?? item.Price;
                        item.Calories = await FetchCaloriesByOfferIdAsync(
                            offer!["id"]!.GetValue<int>(), http);
                        result.Add(item);
                        continue;
                    }

                    foreach (var offer in offers)
                    {
                        var name = offer?["name"]?.GetValue<string>() ?? item.Name;
                        var price = offer?["price"]?.GetValue<decimal>() ?? item.Price;
                        var offerId = offer?["id"]?.GetValue<int>() ?? 0;
                        var calories = offerId > 0
                            ? await FetchCaloriesByOfferIdAsync(offerId, http)
                            : 0;
                        var imageRaw = offer?["image"]?.GetValue<string>() ?? "";
                        var imageUrl = string.IsNullOrEmpty(imageRaw)
                            ? item.ImageUrl
                            : $"https://vkusnoitochka.ru{imageRaw}";
                        result.Add(new ParsedMenuItem
                        {
                            Name = name,
                            Price = price,
                            Calories = calories,
                            CategorySlug = item.CategorySlug,
                            ImageUrl = imageUrl,
                            PageUrl = item.PageUrl,
                            ExternalId = item.ExternalId
                        });

                        await Task.Delay(200);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка разворачивания {item.Name}: {ex.Message}");
                    result.Add(item);
                }
                await Task.Delay(200);
            }
            return result;
        }
        private async Task<int> FetchCaloriesByOfferIdAsync(int offerId, HttpClient http)
        {
            try
            {
                var json = await http.GetStringAsync($"https://vkusnoitochka.ru/api/nutrition/product/{offerId}");
                var doc = JsonNode.Parse(json);
                var amountStr = doc?["nutritionalValue"]?["energyCal"]?["amount"]?.GetValue<string>();
                if (int.TryParse(amountStr, out var cal))
                {
                    return cal;
                }
            }
            catch { }
            return 0;
        }

        public Category? MapCategory(string slug) =>
            CategoryMapper.GetCategory(slug);
    }
}
