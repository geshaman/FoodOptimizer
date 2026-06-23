using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Parser.Helpers
{
    public static class RosticsPdfNameMapper
    {
        public static readonly Dictionary<string, string> PdfToApi =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // PDF-название                              → API-название
                ["шеф баскет оригинальный"] = "шеф баскет со стрипсами",
                ["баскет дуэт оригинальный спешл"] = "баскет дуэт оригинальный",
                ["баскет 11 крыльев + 11 наггетсов"] = "баскет 11 острых крылышек + 11 наггетсов",
            };
        public static string Resolve(string normalizedPdfName)
            => PdfToApi.TryGetValue(normalizedPdfName, out var apiName) ? apiName : normalizedPdfName;
    }
}

