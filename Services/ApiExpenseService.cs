using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Expense service that uses the API backend.
/// </summary>
public class ApiExpenseService : IExpenseService
{
    private readonly ITripMateApiClient _api;

    public ApiExpenseService(ITripMateApiClient api) => _api = api;

    public async Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, int? paidByUserId = null, CancellationToken ct = default) =>
        await _api.GetExpensesByTripAsync(tripId, paidByUserId, ct);

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _api.GetExpenseByIdAsync(id, ct);

    public async Task<Expense> AddAsync(int tripId, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default) =>
        await _api.CreateExpenseAsync(tripId, title, amount, paidByUserId, splitType, note, customSplits, ct);

    public async Task UpdateAsync(int id, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default) =>
        await _api.UpdateExpenseAsync(id, title, amount, paidByUserId, splitType, note, customSplits, ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default) =>
        await _api.DeleteExpenseAsync(id, ct);
}
