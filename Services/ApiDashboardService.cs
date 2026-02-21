namespace TripMate.Services;

/// <summary>
/// Dashboard service that uses the API backend.
/// </summary>
public class ApiDashboardService : IDashboardService
{
    private readonly ITripMateApiClient _api;
    private readonly IAuthService _auth;

    public ApiDashboardService(ITripMateApiClient api, IAuthService auth)
    {
        _api = api;
        _auth = auth;
    }

    public async Task<DashboardSummary> GetTripSummaryAsync(int tripId, CancellationToken ct = default)
    {
        var user = _auth.CurrentUser ?? throw new InvalidOperationException("User not logged in");
        var expenses = await _api.GetExpensesByTripAsync(tripId, null, ct);

        var totalCost = expenses.Sum(e => e.Amount);
        var myPaid = expenses.Where(e => e.PaidByUserId == user.Id).Sum(e => e.Amount);
        var myShare = expenses.SelectMany(e => e.Splits.Where(s => s.UserId == user.Id)).Sum(s => s.ShareAmount);
        var myBalance = myPaid - myShare;

        return new DashboardSummary(totalCost, myPaid, myBalance, expenses.Count);
    }
}
