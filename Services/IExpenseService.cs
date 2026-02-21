using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Expense management service.
/// </summary>
public interface IExpenseService
{
    Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, int? paidByUserId = null, CancellationToken ct = default);
    Task<Expense?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Expense> AddAsync(int tripId, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default);
    Task UpdateAsync(int id, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
