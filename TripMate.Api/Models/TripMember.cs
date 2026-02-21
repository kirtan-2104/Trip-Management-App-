namespace TripMate.Api.Models;

public class TripMember
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public Trip? Trip { get; set; }
    public User? User { get; set; }
}
