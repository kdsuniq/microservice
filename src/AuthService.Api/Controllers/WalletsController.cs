using AuthService.Core.Models;
using AuthService.DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly AuthDbContext _context;

    public WalletsController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallets([FromQuery] Guid userId)
    {
        var wallets = await _context.Wallets
            .Where(w => w.UserId == userId)
            .ToListAsync();
        return Ok(wallets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWallet(Guid id, [FromQuery] Guid userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        
        if (wallet == null)
            return NotFound();
        
        return Ok(wallet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] Wallet wallet)
    {
        // Проверка существует ли пользователь
        var userExists = await _context.Users.AnyAsync(u => u.Id == wallet.UserId);
        if (!userExists)
            return BadRequest("User not found");

        wallet.Id = Guid.NewGuid();
        wallet.CreatedAt = DateTime.UtcNow;
        
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return Ok(wallet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWallet(Guid id, [FromBody] Wallet updatedWallet, [FromQuery] Guid userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        
        if (wallet == null)
            return NotFound();

        wallet.Name = updatedWallet.Name;
        wallet.Currency = updatedWallet.Currency;
        
        await _context.SaveChangesAsync();
        return Ok(wallet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWallet(Guid id, [FromQuery] Guid userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        
        if (wallet == null)
            return NotFound();

        // Проверка есть ли в кошельке транзакции
        var hasTransactions = await _context.Transactions
            .AnyAsync(t => t.WalletId == id);
        
        if (hasTransactions)
            return BadRequest("Невозможно удалить кошелек с существующими транзакциями");

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();

        return Ok();
    }
}