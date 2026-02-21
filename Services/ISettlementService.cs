namespace TripMate.Services;

/// <summary>
/// Represents a settlement instruction: who pays whom how much.
/// </summary>
public record SettlementItem(string PayerName, string ReceiverName, decimal Amount);

/// <summary>
/// Service for computing simplified settlements (who owes whom).
/// </summary>
public interface ISettlementService
{
    Task<IReadOnlyList<SettlementItem>> GetSettlementsAsync(int tripId, CancellationToken ct = default);
}
