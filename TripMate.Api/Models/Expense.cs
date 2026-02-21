namespace TripMate.Api.Models;

public class Expense
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int PaidByUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Note { get; set; }
    public int SplitType { get; set; }
    public Trip? Trip { get; set; }
    public User? PaidByUser { get; set; }
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}
