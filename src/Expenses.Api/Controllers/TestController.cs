using Microsoft.AspNetCore.Mvc;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            Message = "ExpensesService работает!", 
            Service = "Expenses.Api",
            Timestamp = DateTime.UtcNow 
        });
    }

    [HttpGet("trace")]
    public IActionResult GetWithTrace()
    {
        return Ok(new { 
            Message = "TraceId тест ExpensesService",
            Service = "Expenses.Api",
            Timestamp = DateTime.UtcNow
        });
    }
}