using Microsoft.EntityFrameworkCore;
using TripMate.Helpers;
using TripMate.Models;

namespace TripMate.Data;

/// <summary>
/// Seeds the database with sample data for development and demo.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync())
            return; // Already seeded

        var users = new[]
        {
            new User { Name = "Alice Johnson", Email = "alice@example.com", PasswordHash = PasswordHelper.HashPassword("password123") },
            new User { Name = "Bob Smith", Email = "bob@example.com", PasswordHash = PasswordHelper.HashPassword("password123") },
            new User { Name = "Carol Williams", Email = "carol@example.com", PasswordHash = PasswordHelper.HashPassword("password123") },
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var trip = new Trip
        {
            Name = "Goa Beach Trip",
            Destination = "Goa, India",
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(14),
            Status = TripStatus.Upcoming
        };

        await context.Trips.AddAsync(trip);
        await context.SaveChangesAsync();

        var members = users.Select((u, i) => new TripMember { TripId = trip.Id, UserId = u.Id }).ToList();
        await context.TripMembers.AddRangeAsync(members);
        await context.SaveChangesAsync();

        var expense1 = new Expense
        {
            TripId = trip.Id,
            Title = "Hotel Booking",
            Amount = 15000,
            PaidByUserId = users[0].Id,
            SplitType = ExpenseSplitType.Equal,
            Note = "3 nights beach view room"
        };

        await context.Expenses.AddAsync(expense1);
        await context.SaveChangesAsync();

        var equalShare = 15000m / 3;
        foreach (var u in users)
        {
            await context.ExpenseSplits.AddAsync(new ExpenseSplit
            {
                ExpenseId = expense1.Id,
                UserId = u.Id,
                ShareAmount = equalShare
            });
        }

        var expense2 = new Expense
        {
            TripId = trip.Id,
            Title = "Dinner at Beach Shack",
            Amount = 3600,
            PaidByUserId = users[1].Id,
            SplitType = ExpenseSplitType.Equal
        };

        await context.Expenses.AddAsync(expense2);
        await context.SaveChangesAsync();

        foreach (var u in users)
        {
            await context.ExpenseSplits.AddAsync(new ExpenseSplit
            {
                ExpenseId = expense2.Id,
                UserId = u.Id,
                ShareAmount = 1200
            });
        }

        await context.SaveChangesAsync();
    }
}
