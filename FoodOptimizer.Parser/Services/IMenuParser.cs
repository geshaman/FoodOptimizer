using FoodOptimizer.Domain;
using FoodOptimizer.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOptimizer.Parser.Services
{
    public interface IMenuParser
    {
        Task<List<ParsedMenuItem>> ParseMenuAsync();
        Category? MapCategory(string slug);
    }
}
