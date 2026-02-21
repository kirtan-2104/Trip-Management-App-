using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Repository interface for Expense entity operations.
/// </summary>
public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, int? paidByUserId = null, CancellationToken ct = default);
    Task<Expense> AddAsync(Expense expense, IEnumerable<ExpenseSplit> splits, CancellationToken ct = default);
    Task UpdateAsync(Expense expense, IEnumerable<ExpenseSplit> splits, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
