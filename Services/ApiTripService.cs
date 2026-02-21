using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Trip service that uses the API backend.
/// </summary>
public class ApiTripService : ITripService
{
    private readonly ITripMateApiClient _api;
    private readonly IAuthService _auth;

    public ApiTripService(ITripMateApiClient api, IAuthService auth)
    {
        _api = api;
        _auth = auth;
    }

    public async Task<IReadOnlyList<Trip>> GetMyTripsAsync(CancellationToken ct = default)
    {
        var user = _auth.CurrentUser ?? throw new InvalidOperationException("User not logged in");
        return await _api.GetMyTripsAsync(user.Id, ct);
    }

    public async Task<Trip?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _api.GetTripByIdAsync(id, ct);

    public async Task<Trip> CreateAsync(string name, string destination, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var user = _auth.CurrentUser ?? throw new InvalidOperationException("User not logged in");
        return await _api.CreateTripAsync(name, destination, startDate, endDate, user.Id, ct);
    }

    public async Task UpdateAsync(int id, string name, string destination, DateTime startDate, DateTime endDate, TripStatus status, CancellationToken ct = default) =>
        await _api.UpdateTripAsync(id, name, destination, startDate, endDate, status, ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default) =>
        await _api.DeleteTripAsync(id, ct);

    public async Task AddMemberAsync(int tripId, int userId, CancellationToken ct = default) =>
        await _api.AddTripMemberAsync(tripId, userId, ct);

    public async Task RemoveMemberAsync(int tripId, int userId, CancellationToken ct = default) =>
        await _api.RemoveTripMemberAsync(tripId, userId, ct);

    public async Task<IReadOnlyList<User>> GetMembersAsync(int tripId, CancellationToken ct = default) =>
        await _api.GetTripMembersAsync(tripId, ct);
}
