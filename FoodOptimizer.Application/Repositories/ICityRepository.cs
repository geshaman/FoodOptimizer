using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FoodOptimizer.Application.Repositories
{
    public interface ICityRepository
    {
        public Task<List<string>> GetCitiesAsync();
    }
}
