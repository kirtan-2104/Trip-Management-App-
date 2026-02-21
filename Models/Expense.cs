namespace TripMate.Models;

/// <summary>
/// Represents an expense within a trip.
/// </summary>
public class Expense
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int PaidByUserId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public ExpenseSplitType SplitType { get; set; }

    // Navigation properties
    public Trip Trip { get; set; } = null!;
    public User PaidByUser { get; set; } = null!;
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}

/// <summary>
/// How an expense is split among members.
/// </summary>
public enum ExpenseSplitType
{
    Equal = 0,
    Custom = 1
}
