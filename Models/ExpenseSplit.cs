namespace TripMate.Models;

/// <summary>
/// Represents a single person's share of an expense.
/// Used for both equal and custom splits.
/// </summary>
public class ExpenseSplit
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public int UserId { get; set; }
    public decimal ShareAmount { get; set; }

    // Navigation properties
    public Expense Expense { get; set; } = null!;
    public User User { get; set; } = null!;
}
