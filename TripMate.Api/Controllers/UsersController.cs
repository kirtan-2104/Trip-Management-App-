using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripMate.Api.Data;
using TripMate.Api.DTOs;
using TripMate.Api.Helpers;
using TripMate.Api.Models;

namespace TripMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
        Ok(await _db.Users.ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(CreateUserRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest("Email already exists");
        var user = new User
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = PasswordHelper.Hash(req.Password),
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null || !PasswordHelper.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");
        return Ok(user);
    }
}
