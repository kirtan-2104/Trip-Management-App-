using Microsoft.EntityFrameworkCore;
using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Repository implementation for Trip entity.
/// </summary>
public class TripRepository : ITripRepository
{
    private readonly AppDbContext _context;

    public TripRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Trip?> GetByIdAsync(int id, bool includeMembers = false, bool includeExpenses = false, CancellationToken ct = default)
    {
        var query = _context.Trips.AsQueryable();
        if (includeMembers) query = query.Include(t => t.Members).ThenInclude(m => m.User);
        if (includeExpenses) query = query.Include(t => t.Expenses).ThenInclude(e => e.PaidByUser).Include(t => t.Expenses).ThenInclude(e => e.Splits);
        return await query.FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Trips.Include(t => t.Members).ThenInclude(m => m.User).OrderByDescending(t => t.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(int userId, CancellationToken ct = default) =>
        await _context.Trips
            .Include(t => t.Members).ThenInclude(m => m.User)
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

    public async Task<Trip> AddAsync(Trip trip, CancellationToken ct = default)
    {
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync(ct);
        return trip;
    }

    public async Task UpdateAsync(Trip trip, CancellationToken ct = default)
    {
        _context.Trips.Update(trip);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var trip = await _context.Trips.FindAsync([id], ct);
        if (trip != null)
        {
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<TripMember>> GetMembersAsync(int tripId, CancellationToken ct = default) =>
        await _context.TripMembers.Include(m => m.User).Where(m => m.TripId == tripId).ToListAsync(ct);

    public async Task AddMemberAsync(TripMember member, CancellationToken ct = default)
    {
        _context.TripMembers.Add(member);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(int tripId, int userId, CancellationToken ct = default)
    {
        var member = await _context.TripMembers.FirstOrDefaultAsync(m => m.TripId == tripId && m.UserId == userId, ct);
        if (member != null)
        {
            _context.TripMembers.Remove(member);
            await _context.SaveChangesAsync(ct);
        }
    }
}
