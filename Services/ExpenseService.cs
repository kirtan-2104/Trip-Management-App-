using TripMate.Data;
using TripMate.Models;

namespace TripMate.Services;

/// <summary>
/// Expense service implementation.
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IExpenseCalculationService _calculationService;

    public ExpenseService(IExpenseRepository expenseRepository, ITripRepository tripRepository, IExpenseCalculationService calculationService)
    {
        _expenseRepository = expenseRepository;
        _tripRepository = tripRepository;
        _calculationService = calculationService;
    }

    public async Task<IReadOnlyList<Expense>> GetByTripIdAsync(int tripId, int? paidByUserId = null, CancellationToken ct = default) =>
        await _expenseRepository.GetByTripIdAsync(tripId, paidByUserId, ct);

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _expenseRepository.GetByIdAsync(id, ct);

    public async Task<Expense> AddAsync(int tripId, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default)
    {
        var members = await _tripRepository.GetMembersAsync(tripId, ct);
        var userIds = members.Select(m => m.Id).ToList();

        var expense = new Expense
        {
            TripId = tripId,
            Title = title,
            Amount = amount,
            PaidByUserId = paidByUserId,
            SplitType = splitType,
            Note = note
        };

        IEnumerable<ExpenseSplit> splits;
        if (splitType == ExpenseSplitType.Custom && customSplits != null && customSplits.Count > 0)
        {
            splits = customSplits.Select(kv => new ExpenseSplit
            {
                UserId = kv.Key,
                ShareAmount = kv.Value
            });
        }
        else
        {
            splits = _calculationService.CalculateEqualSplit(0, amount, userIds);
        }

        return await _expenseRepository.AddAsync(expense, splits.ToList(), ct);
    }

    public async Task UpdateAsync(int id, string title, decimal amount, int paidByUserId, ExpenseSplitType splitType, string? note, IReadOnlyDictionary<int, decimal>? customSplits, CancellationToken ct = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Expense not found");
        var members = await _tripRepository.GetMembersAsync(expense.TripId, ct);
        var userIds = members.Select(m => m.Id).ToList();

        expense.Title = title;
        expense.Amount = amount;
        expense.PaidByUserId = paidByUserId;
        expense.SplitType = splitType;
        expense.Note = note;

        IEnumerable<ExpenseSplit> splits;
        if (splitType == ExpenseSplitType.Custom && customSplits != null && customSplits.Count > 0)
        {
            splits = customSplits.Select(kv => new ExpenseSplit
            {
                ExpenseId = id,
                UserId = kv.Key,
                ShareAmount = kv.Value
            });
        }
        else
        {
            splits = _calculationService.CalculateEqualSplit(id, amount, userIds);
        }

        await _expenseRepository.UpdateAsync(expense, splits.ToList(), ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default) =>
        await _expenseRepository.DeleteAsync(id, ct);
}
