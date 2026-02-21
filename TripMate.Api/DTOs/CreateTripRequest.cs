namespace TripMate.Api.DTOs;

public record CreateTripRequest(string Name, string Destination, DateTime StartDate, DateTime EndDate, int UserId);
