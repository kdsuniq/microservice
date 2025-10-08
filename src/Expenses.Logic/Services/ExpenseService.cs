using CoreLib.Models;
using Expenses.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Logic.Services;

public class ExpenseService
{
    private readonly ExpensesDbContext _context;

    public ExpenseService(ExpensesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expense>> GetAllAsync(Guid userId)
    {
        return await _context.Expenses
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _context.Expenses.FindAsync(id);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        expense.Id = Guid.NewGuid();
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        var exists = await _context.Expenses.AnyAsync(x => x.Id == expense.Id);
        if (!exists) return false;
        
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Expenses.FindAsync(id);
        if (entity == null) return false;
        
        _context.Expenses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}