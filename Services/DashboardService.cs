using TripMate.Data;

namespace TripMate.Services;

/// <summary>
/// Dashboard service implementation.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IAuthService _authService;

    public DashboardService(IExpenseRepository expenseRepository, ITripRepository tripRepository, IAuthService authService)
    {
        _expenseRepository = expenseRepository;
        _tripRepository = tripRepository;
        _authService = authService;
    }

    public async Task<DashboardSummary> GetTripSummaryAsync(int tripId, CancellationToken ct = default)
    {
        var user = _authService.CurrentUser ?? throw new InvalidOperationException("User not logged in");
        var expenses = await _expenseRepository.GetByTripIdAsync(tripId, null, ct);

        var totalCost = expenses.Sum(e => e.Amount);
        var myPaid = expenses.Where(e => e.PaidByUserId == user.Id).Sum(e => e.Amount);
        var myShare = expenses.SelectMany(e => e.Splits.Where(s => s.UserId == user.Id)).Sum(s => s.ShareAmount);
        var myBalance = myPaid - myShare;

        return new DashboardSummary(totalCost, myPaid, myBalance, expenses.Count);
    }
}
