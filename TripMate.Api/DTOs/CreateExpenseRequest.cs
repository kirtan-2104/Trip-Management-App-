namespace TripMate.Api.DTOs;

public record CreateExpenseRequest(int TripId, string Title, decimal Amount, int PaidByUserId, int SplitType, string? Note, Dictionary<int, decimal>? CustomSplits);
