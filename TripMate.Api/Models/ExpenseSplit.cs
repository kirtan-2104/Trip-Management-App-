namespace TripMate.Api.Models;

public class ExpenseSplit
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public int UserId { get; set; }
    public decimal ShareAmount { get; set; }
    public Expense? Expense { get; set; }
    public User? User { get; set; }
}
