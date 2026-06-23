using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
namespace FoodOptimizer.Parser.Services
{
    public class RosticsPdfCaloriesParser
    {
        private static readonly Regex DishLineRegex = new(
            @"^БЛЮДО\s+(.+?)\s+ТТК\s+\S+\s+\d+(?:,\d+)?\s+[\d,]+\s+[\d,]+\s+[\d,]+\s+([\d,]+)",
            RegexOptions.Compiled);

        public async Task<Dictionary<string, int>> ParseFromUrlAsync(string pdfUrl)
        {
            using var httpClient = new HttpClient();
            var pdfBytes = await httpClient.GetByteArrayAsync(pdfUrl);
            return ParseFromBytes(pdfBytes);
        }

        public Dictionary<string, int> ParseFromBytes(byte[] pdfBytes)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            using var document = PdfDocument.Open(pdfBytes);

            foreach (var page in document.GetPages())
            {
                // PdfPig возвращает слова с координатами — собираем в строки
                var lines = ExtractLines(page);

                foreach (var line in lines)
                {
                    var match = DishLineRegex.Match(line);
                    if (!match.Success) continue;

                    var rawName = match.Groups[1].Value.Trim();
                    var caloriesStr = match.Groups[2].Value.Replace(',', '.');

                    if (!int.TryParse(caloriesStr.Split('.')[0], out var calories))
                        continue;

                    if (calories <= 0) continue;

                    // Нормализуем имя для поиска
                    var normalizedName = NormalizeName(rawName);
                    result.TryAdd(normalizedName, calories);
                }
            }

            return result;
        }

        private static List<string> ExtractLines(Page page)
        {
            const double lineToleranceY = 3.0; // пикселей — слова на одной строке

            var wordsByLine = page.GetWords()
                .GroupBy(w => Math.Round(w.BoundingBox.Bottom / lineToleranceY) * lineToleranceY)
                .OrderByDescending(g => g.Key); // PDF: Y растёт снизу вверх

            return wordsByLine
                .Select(g => string.Join(" ", g.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text)))
                .ToList();
        }

        public static string NormalizeName(string name)
            => Regex.Replace(name.ToLowerInvariant().Trim(), @"\s+", " ");
    }
}
