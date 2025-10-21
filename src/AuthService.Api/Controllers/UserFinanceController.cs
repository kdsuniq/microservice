using AuthService.Core.Infrastructure;
using AuthService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserFinanceController : ControllerBase
    {
        private readonly HttpService _httpService;

        public UserFinanceController(HttpService httpService)
        {
            _httpService = httpService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                // Вызов сервиса Б (Expenses.Api)
                var categories = await _httpService.GetAsync<List<Category>>("api/Categories");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = "Не удалось получить категории", 
                    details = ex.Message, 
                    traceId = _httpService.GetTraceId() 
                });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("UserFinance controller is working");
        }
    }
}