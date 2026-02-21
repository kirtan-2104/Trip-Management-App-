using Microsoft.EntityFrameworkCore;
using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Repository implementation for Expense entity.
/// </summary>
public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Expenses
            .Include(e => e.PaidByUser)
            .Include(e => e.Splits).ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, int? paidByUserId = null, CancellationToken ct = default)
    {
        var query = _context.Expenses
            .Include(e => e.PaidByUser)
            .Include(e => e.Splits).ThenInclude(s => s.User)
            .Where(e => e.TripId == tripId);

        if (paidByUserId.HasValue)
            query = query.Where(e => e.PaidByUserId == paidByUserId.Value);

        return await query.OrderByDescending(e => e.CreatedDate).ToListAsync(ct);
    }

    public async Task<Expense> AddAsync(Expense expense, IEnumerable<ExpenseSplit> splits, CancellationToken ct = default)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(ct);

        foreach (var split in splits)
        {
            split.ExpenseId = expense.Id;
            _context.ExpenseSplits.Add(split);
        }
        await _context.SaveChangesAsync(ct);
        return expense;
    }

    public async Task UpdateAsync(Expense expense, IEnumerable<ExpenseSplit> splits, CancellationToken ct = default)
    {
        var existingSplits = await _context.ExpenseSplits.Where(s => s.ExpenseId == expense.Id).ToListAsync(ct);
        _context.ExpenseSplits.RemoveRange(existingSplits);
        _context.Expenses.Update(expense);

        foreach (var split in splits)
        {
            split.ExpenseId = expense.Id;
            _context.ExpenseSplits.Add(split);
        }
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var expense = await _context.Expenses.FindAsync([id], ct);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync(ct);
        }
    }
}
