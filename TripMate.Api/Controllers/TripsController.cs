using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripMate.Api.Data;
using TripMate.Api.DTOs;
using TripMate.Api.Models;

namespace TripMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TripsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trip>>> GetAll() =>
        Ok(await _db.Trips.Include(t => t.Members).ToListAsync());

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<Trip>>> GetByUser(int userId) =>
        Ok(await _db.Trips
            .Include(t => t.Members).ThenInclude(m => m.User)
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Trip>> GetById(int id)
    {
        var trip = await _db.Trips
            .Include(t => t.Members).ThenInclude(m => m.User)
            .Include(t => t.Expenses).ThenInclude(e => e.PaidByUser)
            .Include(t => t.Expenses).ThenInclude(e => e.Splits).ThenInclude(s => s.User)
            .FirstOrDefaultAsync(t => t.Id == id);
        return trip == null ? NotFound() : Ok(trip);
    }

    [HttpPost]
    public async Task<ActionResult<Trip>> Create(CreateTripRequest req)
    {
        var trip = new Trip
        {
            Name = req.Name,
            Destination = req.Destination,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            Status = req.StartDate > DateTime.UtcNow ? 0 : (req.EndDate < DateTime.UtcNow ? 2 : 1),
            CreatedAt = DateTime.UtcNow
        };
        _db.Trips.Add(trip);
        await _db.SaveChangesAsync();
        _db.TripMembers.Add(new TripMember { TripId = trip.Id, UserId = req.UserId, JoinedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTripRequest req)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip == null) return NotFound();
        trip.Name = req.Name;
        trip.Destination = req.Destination;
        trip.StartDate = req.StartDate;
        trip.EndDate = req.EndDate;
        trip.Status = req.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trip = await _db.Trips.FindAsync(id);
        if (trip == null) return NotFound();
        _db.Trips.Remove(trip);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:int}/members")]
    public async Task<ActionResult<IEnumerable<User>>> GetMembers(int id)
    {
        var members = await _db.TripMembers.Include(m => m.User)
            .Where(m => m.TripId == id).Select(m => m.User).ToListAsync();
        return Ok(members);
    }

    [HttpPost("members")]
    public async Task<IActionResult> AddMember(AddMemberRequest req)
    {
        if (await _db.TripMembers.AnyAsync(m => m.TripId == req.TripId && m.UserId == req.UserId))
            return BadRequest("Already a member");
        _db.TripMembers.Add(new TripMember { TripId = req.TripId, UserId = req.UserId, JoinedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{tripId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int tripId, int userId)
    {
        var m = await _db.TripMembers.FirstOrDefaultAsync(x => x.TripId == tripId && x.UserId == userId);
        if (m == null) return NotFound();
        _db.TripMembers.Remove(m);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
