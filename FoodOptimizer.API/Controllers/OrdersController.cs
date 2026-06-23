using FoodOptimizer.Application.DTOs;
using FoodOptimizer.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace FoodOptimizer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderOptimizerService _optimizerService;

        public OrdersController(IOrderOptimizerService optimizerService)
        {
            _optimizerService = optimizerService;
        }

        [HttpPost("optimize")]
        public async Task<ActionResult> Optimize([FromBody] OptimizationRequest request)
        {
            try
            {
                var variants = await _optimizerService.GetOrderVariantsAsync(request);
                return Ok(variants);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}