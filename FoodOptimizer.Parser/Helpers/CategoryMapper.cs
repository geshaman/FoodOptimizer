using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOptimizer.Domain;
namespace FoodOptimizer.Parser.Helpers
{
    public static class CategoryMapper
    {
        private static readonly Dictionary<string, Category> Map = new()
        {
            { "novinki",              Category.Entree },
            { "burgery-i-rolly",      Category.Entree },
            { "vse-burgery-i-rolly",  Category.Entree },
            { "napitki",              Category.Drinks },
            { "vse-napitki",          Category.Drinks },
            { "goryachie-napitki",    Category.Drinks },
            { "prokhladitelnye-napitki", Category.Drinks },
            { "kartofel-i-sneki",     Category.Garnish },
            { "vse-kartofel-i-sneki", Category.Garnish },
            { "kartofel_1",           Category.Garnish },
            { "sneki",                Category.Garnish },
            { "deserty_4",            Category.Dessert },
            { "vse-deserty",          Category.Dessert },
            { "pirozhok_1",           Category.Dessert },
            { "torty-i-pirozhnye",    Category.Dessert },
            { "kombo-obed_2",         Category.Combo },
            { "vse-kombo-obed",       Category.Combo },
            { "sety-i-pary_1",        Category.Combo },
            { "vse-sety-i-pary",      Category.Combo },
            { "mega-kombo",           Category.Combo },
            { "kidz-kombo",           Category.Combo },
            { "kafe",                 Category.Drinks },
            { "vse-kafe",             Category.Drinks },
            { "sousy",                Category.Sauces },
            { "tolko-v-dostavke",     Category.Entree },
            { "govyadina",            Category.Entree },
            { "govyadina_2",          Category.Entree },
            { "kuritsa",              Category.Entree },
            { "kuritsa_2",            Category.Entree },
            { "ovoshchi-i-frukty",    Category.Garnish },
            { "Бургеры",        Category.Entree },
            { "Новинки",        Category.Entree },
            { "Воппер",         Category.Entree },
            { "Курица",         Category.Entree },
            { "Снеки",          Category.Garnish },
            { "Картофель",      Category.Garnish },
            { "Напитки",        Category.Drinks },
            { "Десерты",        Category.Dessert },
            { "Соусы",          Category.Sauces },
            { "Комбо",          Category.Combo },
            { "Детское меню",   Category.Combo },
            { "Роллы",            Category.Entree },
            { "Кинг Комбо",              Category.Combo },
            { "Вкусы Воппера",           Category.Entree },
            { "Love Is...",              Category.Entree },
            { "Картофель и закуски",     Category.Garnish },
            { "Напитки и десерты",       Category.Drinks },
            { "Соусы и другие товары",   Category.Sauces },
            { "Купоны",                  Category.Combo },
        };

        public static Category? GetCategory(string slug)
        {
            return Map.TryGetValue(slug, out var category) ? category : null;
        }

        public static Category? GetCategoryByTitle(string title)
        {
            if (title.Contains("Комбо", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Микс", StringComparison.OrdinalIgnoreCase))
                return Category.Combo;
            if (title.Contains("Сырные палочки", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Кревет", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Баскет фри", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Картофель", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Дольки", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Пати бокс", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Байтсы", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Наггетсы", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Наггетс бокс", StringComparison.OrdinalIgnoreCase))
                return Category.Garnish;
            if (title.Contains("Бургер", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Сэндвич", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Баскет", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Ростмастер", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Ролл", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Дабл чикен", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Стрипс", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Крылыш", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Ножк", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Маэстро", StringComparison.OrdinalIgnoreCase))
                return Category.Entree;
            if (title.Contains("Лимонад", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Бабл", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Кола", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Чай", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Эвервесс", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Фрустайл", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Пиво", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Аква Минерале", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Сок", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Милкшейк", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Кофе", StringComparison.OrdinalIgnoreCase))
                return Category.Drinks;
            if (title.Contains("Чизкейк", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Маффин", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Мини-тарт", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Донат", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Пирожок", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Мороженое", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Айс Микс", StringComparison.OrdinalIgnoreCase))
                return Category.Dessert;
            if (title.Contains("Соус", StringComparison.OrdinalIgnoreCase)
                || title.Contains("Кетчуп", StringComparison.OrdinalIgnoreCase))
                return Category.Sauces;
            return null;
        }
    }
}
