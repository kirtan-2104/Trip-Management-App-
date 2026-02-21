namespace TripMate.Api.DTOs;

public record UpdateTripRequest(string Name, string Destination, DateTime StartDate, DateTime EndDate, int Status);
