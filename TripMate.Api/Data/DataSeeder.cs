using Microsoft.EntityFrameworkCore;
using TripMate.Api.Helpers;
using TripMate.Api.Models;

namespace TripMate.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Users.AnyAsync()) return;

        var users = new[]
        {
            new User { Name = "Alice Johnson", Email = "alice@example.com", PasswordHash = PasswordHelper.Hash("password123"), CreatedAt = DateTime.UtcNow },
            new User { Name = "Bob Smith", Email = "bob@example.com", PasswordHash = PasswordHelper.Hash("password123"), CreatedAt = DateTime.UtcNow },
            new User { Name = "Carol Williams", Email = "carol@example.com", PasswordHash = PasswordHelper.Hash("password123"), CreatedAt = DateTime.UtcNow },
        };
        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        var trip = new Trip
        {
            Name = "Goa Beach Trip",
            Destination = "Goa, India",
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(14),
            Status = 0,
            CreatedAt = DateTime.UtcNow
        };
        db.Trips.Add(trip);
        await db.SaveChangesAsync();

        foreach (var u in users)
            db.TripMembers.Add(new TripMember { TripId = trip.Id, UserId = u.Id, JoinedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var exp1 = new Expense { TripId = trip.Id, Title = "Hotel Booking", Amount = 15000, PaidByUserId = users[0].Id, SplitType = 0, Note = "3 nights", CreatedDate = DateTime.UtcNow };
        db.Expenses.Add(exp1);
        await db.SaveChangesAsync();
        foreach (var u in users)
            db.ExpenseSplits.Add(new ExpenseSplit { ExpenseId = exp1.Id, UserId = u.Id, ShareAmount = 5000 });
        await db.SaveChangesAsync();

        var exp2 = new Expense { TripId = trip.Id, Title = "Dinner at Beach Shack", Amount = 3600, PaidByUserId = users[1].Id, SplitType = 0, CreatedDate = DateTime.UtcNow };
        db.Expenses.Add(exp2);
        await db.SaveChangesAsync();
        foreach (var u in users)
            db.ExpenseSplits.Add(new ExpenseSplit { ExpenseId = exp2.Id, UserId = u.Id, ShareAmount = 1200 });
        await db.SaveChangesAsync();
    }
}
