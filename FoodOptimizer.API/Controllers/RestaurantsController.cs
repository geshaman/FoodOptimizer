using FoodOptimizer.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FoodOptimizer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantRepository _repository;

        public RestaurantsController(IRestaurantRepository repository)
        {
            _repository = repository;
        }
    }
}