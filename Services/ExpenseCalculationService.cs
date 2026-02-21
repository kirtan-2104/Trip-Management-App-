using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Expense calculation service implementation.
/// Handles equal and custom split calculations.
/// </summary>
public class ExpenseCalculationService : IExpenseCalculationService
{
    public IReadOnlyList<ExpenseSplit> CalculateEqualSplit(int expenseId, decimal amount, IReadOnlyList<int> userIds)
    {
        if (userIds.Count == 0)
            return Array.Empty<ExpenseSplit>();

        var share = Math.Round(amount / userIds.Count, 2);
        var remainder = amount - (share * userIds.Count);

        var splits = new List<ExpenseSplit>();
        for (var i = 0; i < userIds.Count; i++)
        {
            var shareAmount = share + (i == 0 ? remainder : 0);
            splits.Add(new ExpenseSplit
            {
                ExpenseId = expenseId,
                UserId = userIds[i],
                ShareAmount = shareAmount
            });
        }
        return splits;
    }

    public decimal GetPerPersonAmountEqual(decimal totalAmount, int memberCount) =>
        memberCount > 0 ? Math.Round(totalAmount / memberCount, 2) : 0;
}
