namespace TripMate.Models;

/// <summary>
/// Represents a group trip with members and expenses.
/// </summary>
public class Trip
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TripStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<TripMember> Members { get; set; } = new List<TripMember>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}

/// <summary>
/// Trip status enumeration.
/// </summary>
public enum TripStatus
{
    Upcoming = 0,
    Ongoing = 1,
    Completed = 2
}
