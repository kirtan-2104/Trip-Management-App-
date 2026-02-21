namespace TripMate.Services;

/// <summary>
/// Dashboard summary data for a trip.
/// </summary>
public record DashboardSummary(
    decimal TotalTripCost,
    decimal YourTotalPaid,
    decimal YourBalance,
    int ExpenseCount
);

/// <summary>
/// Service for dashboard and report aggregation.
/// </summary>
public interface IDashboardService
{
    Task<DashboardSummary> GetTripSummaryAsync(int tripId, CancellationToken ct = default);
}
