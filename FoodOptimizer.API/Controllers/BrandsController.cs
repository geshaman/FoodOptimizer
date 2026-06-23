using FoodOptimizer.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FoodOptimizer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _repository;

        public BrandsController(IBrandRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrandsByCity([FromQuery] string city)
        {
            try
            {
                var result = await _repository.GetBrandsByCityAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}