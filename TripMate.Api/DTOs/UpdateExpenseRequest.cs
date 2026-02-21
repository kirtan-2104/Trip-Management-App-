namespace TripMate.Api.DTOs;

public record UpdateExpenseRequest(string Title, decimal Amount, int PaidByUserId, int SplitType, string? Note, Dictionary<int, decimal>? CustomSplits);
