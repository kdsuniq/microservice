using AuthService.Core.Models;
using AuthService.DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AuthDbContext _context;

    public TransactionsController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] Guid userId,
        [FromQuery] Guid? walletId = null,
        [FromQuery] TransactionType? type = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Wallet)
            .AsQueryable();

        if (walletId.HasValue)
            query = query.Where(t => t.WalletId == walletId.Value);
        
        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);
        
        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var transactions = await query
            .OrderByDescending(t => t.Date)
            .ToListAsync();
        
        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // проверка существует ли кошелек и принадлежит ли он пользователю
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == transaction.WalletId && w.UserId == transaction.UserId);
            
            if (wallet == null)
                return BadRequest("Кошелек не найден");

            transaction.Id = Guid.NewGuid();
            transaction.Date = DateTime.UtcNow;
            
            _context.Transactions.Add(transaction);

            // Обновление баланса 
            if (transaction.Type == TransactionType.Income)
                wallet.Balance += transaction.Amount;
            else
                wallet.Balance -= transaction.Amount;

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return Ok(transaction);
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] Transaction updatedTransaction, [FromQuery] Guid userId)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            
            if (transaction == null)
                return NotFound();

            var oldAmount = transaction.Amount;
            var oldType = transaction.Type;

            // Обновление транзакций 
            if (oldType == TransactionType.Income)
                transaction.Wallet.Balance -= oldAmount;
            else
                transaction.Wallet.Balance += oldAmount;

            // Добавляем новые транзакции 
            transaction.Amount = updatedTransaction.Amount;
            transaction.Description = updatedTransaction.Description;
            transaction.Type = updatedTransaction.Type;
            transaction.Date = updatedTransaction.Date;

            if (transaction.Type == TransactionType.Income)
                transaction.Wallet.Balance += transaction.Amount;
            else
                transaction.Wallet.Balance -= transaction.Amount;

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return Ok(transaction);
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id, [FromQuery] Guid userId)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            
            if (transaction == null)
                return NotFound();

            // Влияние отмены транзакции на баланс кошелька
            if (transaction.Type == TransactionType.Income)
                transaction.Wallet.Balance -= transaction.Amount;
            else
                transaction.Wallet.Balance += transaction.Amount;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return Ok();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}