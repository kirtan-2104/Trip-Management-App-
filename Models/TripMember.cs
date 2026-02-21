namespace TripMate.Models;

/// <summary>
/// Junction table linking users to trips (many-to-many).
/// </summary>
public class TripMember
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Trip Trip { get; set; } = null!;
    public User User { get; set; } = null!;
}
