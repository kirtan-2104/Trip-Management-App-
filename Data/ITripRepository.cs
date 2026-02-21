using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Repository interface for Trip entity operations.
/// </summary>
public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(int id, bool includeMembers = false, bool includeExpenses = false, CancellationToken ct = default);
    Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Trip>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Trip> AddAsync(Trip trip, CancellationToken ct = default);
    Task UpdateAsync(Trip trip, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TripMember>> GetMembersAsync(int tripId, CancellationToken ct = default);
    Task AddMemberAsync(TripMember member, CancellationToken ct = default);
    Task RemoveMemberAsync(int tripId, int userId, CancellationToken ct = default);
}
