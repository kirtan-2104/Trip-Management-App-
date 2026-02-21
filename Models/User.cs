namespace TripMate.Models;

/// <summary>
/// Represents a user in the TripMate application.
/// Stored locally for authentication and trip membership.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties for EF Core
    public ICollection<TripMember> TripMemberships { get; set; } = new List<TripMember>();
}
