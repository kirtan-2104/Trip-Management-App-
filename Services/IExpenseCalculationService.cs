using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Service for calculating expense splits and per-person amounts.
/// </summary>
public interface IExpenseCalculationService
{
    IReadOnlyList<ExpenseSplit> CalculateEqualSplit(int expenseId, decimal amount, IReadOnlyList<int> userIds);
    decimal GetPerPersonAmountEqual(decimal totalAmount, int memberCount);
}
