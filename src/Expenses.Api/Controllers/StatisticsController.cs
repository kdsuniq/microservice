using Microsoft.AspNetCore.Mvc;
using Expenses.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly ExpensesDbContext _context;

    public StatisticsController(ExpensesDbContext context)
    {
        _context = context;
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeekly([FromQuery] Guid userId)
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        
        var stats = await _context.Expenses
            .Where(x => x.UserId == userId && x.Date >= startDate)
            .GroupBy(x => x.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalAmount = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .ToListAsync();

        return Ok(stats);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] Guid userId)
    {
        var startDate = DateTime.UtcNow.AddDays(-30);
        
        var stats = await _context.Expenses
            .Where(x => x.UserId == userId && x.Date >= startDate)
            .GroupBy(x => x.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalAmount = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .ToListAsync();

        return Ok(stats);
    }
}