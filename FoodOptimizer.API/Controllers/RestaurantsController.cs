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

        [HttpGet]
        public async Task<IActionResult> GetByCity([FromQuery] string city)
        {
            try
            {
                var restaurants = await _repository.GetRestaurantsByCityAsync(city);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}