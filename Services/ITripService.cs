using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Trip management service.
/// </summary>
public interface ITripService
{
    Task<IReadOnlyList<Trip>> GetMyTripsAsync(CancellationToken ct = default);
    Task<Trip?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Trip> CreateAsync(string name, string destination, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task UpdateAsync(int id, string name, string destination, DateTime startDate, DateTime endDate, TripStatus status, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task AddMemberAsync(int tripId, int userId, CancellationToken ct = default);
    Task RemoveMemberAsync(int tripId, int userId, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetMembersAsync(int tripId, CancellationToken ct = default);
}
