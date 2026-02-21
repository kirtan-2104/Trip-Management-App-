using TripMate.Data;
using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Trip service implementation.
/// </summary>
public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAuthService _authService;

    public TripService(ITripRepository tripRepository, IAuthService authService)
    {
        _tripRepository = tripRepository;
        _authService = authService;
    }

    public async Task<IReadOnlyList<Trip>> GetMyTripsAsync(CancellationToken ct = default)
    {
        var user = _authService.CurrentUser ?? throw new InvalidOperationException("User not logged in");
        return await _tripRepository.GetByUserIdAsync(user.Id, ct);
    }

    public async Task<Trip?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _tripRepository.GetByIdAsync(id, includeMembers: true, includeExpenses: true, ct);

    public async Task<Trip> CreateAsync(string name, string destination, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var user = _authService.CurrentUser ?? throw new InvalidOperationException("User not logged in");

        var trip = new Trip
        {
            Name = name,
            Destination = destination,
            StartDate = startDate,
            EndDate = endDate,
            Status = DetermineStatus(startDate, endDate)
        };

        trip = await _tripRepository.AddAsync(trip, ct);
        await _tripRepository.AddMemberAsync(new TripMember { TripId = trip.Id, UserId = user.Id }, ct);
        return trip;
    }

    public async Task UpdateAsync(int id, string name, string destination, DateTime startDate, DateTime endDate, TripStatus status, CancellationToken ct = default)
    {
        var trip = await _tripRepository.GetByIdAsync(id, false, false, ct) ?? throw new InvalidOperationException("Trip not found");
        trip.Name = name;
        trip.Destination = destination;
        trip.StartDate = startDate;
        trip.EndDate = endDate;
        trip.Status = status;
        await _tripRepository.UpdateAsync(trip, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default) =>
        await _tripRepository.DeleteAsync(id, ct);

    public async Task AddMemberAsync(int tripId, int userId, CancellationToken ct = default) =>
        await _tripRepository.AddMemberAsync(new TripMember { TripId = tripId, UserId = userId }, ct);

    public async Task RemoveMemberAsync(int tripId, int userId, CancellationToken ct = default) =>
        await _tripRepository.RemoveMemberAsync(tripId, userId, ct);

    public async Task<IReadOnlyList<User>> GetMembersAsync(int tripId, CancellationToken ct = default)
    {
        var members = await _tripRepository.GetMembersAsync(tripId, ct);
        return members.Select(m => m.User).ToList();
    }

    private static TripStatus DetermineStatus(DateTime start, DateTime end)
    {
        var now = DateTime.UtcNow.Date;
        if (now < start) return TripStatus.Upcoming;
        if (now > end) return TripStatus.Completed;
        return TripStatus.Ongoing;
    }
}
