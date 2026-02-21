namespace TripMate.Services;

/// <summary>
/// Settlement service that uses the API backend.
/// </summary>
public class ApiSettlementService : ISettlementService
{
    private readonly ITripMateApiClient _api;

    public ApiSettlementService(ITripMateApiClient api) => _api = api;

    public async Task<IReadOnlyList<SettlementItem>> GetSettlementsAsync(int tripId, CancellationToken ct = default) =>
        await _api.GetSettlementsAsync(tripId, ct);
}
