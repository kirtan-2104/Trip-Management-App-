using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// API client for TripMate backend.
/// </summary>
public interface ITripMateApiClient
{
    Task<(User? User, string Message)> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<(bool Success, string Message)> RegisterAsync(string name, string email, string password, CancellationToken ct = default);

    Task<IReadOnlyList<Trip>> GetMyTripsAsync(int userId, CancellationToken ct = default);
    Task<Trip?> GetTripByIdAsync(int id, CancellationToken ct = default);
    Task<Trip> CreateTripAsync(string name, string destination, DateTime startDate, DateTime endDate, int userId, CancellationToken ct = default);
    Task UpdateTripAsync(int id, string name, string destination, DateTime startDate, DateTime endDate, TripStatus status, CancellationToken ct = default);
    Task DeleteTripAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetTripMembersAsync(int tripId, CancellationToken ct = default);
    Task AddTripMemberAsync(int tripId, int userId, CancellationToken ct = default);
    Task RemoveTripMemberAsync(int tripId, int userId, CancellationToken ct = default);

    Task<IReadOnlyList<Expense>> GetExpensesByTripAsync(int tripId, int? paidByUserId, CancellationToken ct = default);
    Task<Expense?> GetExpenseByIdAsync(int id, CancellationToken ct = default);
    Task<Expense> CreateExpenseAsync(int tripId, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default);
    Task UpdateExpenseAsync(int id, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default);
    Task DeleteExpenseAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<SettlementItem>> GetSettlementsAsync(int tripId, CancellationToken ct = default);
}
